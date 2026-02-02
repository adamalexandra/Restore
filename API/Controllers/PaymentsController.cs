using System;
using API.Data;
using API.DTOs;
using API.Entities.OrderAggregate;
using API.Extentions;
using API.Servieces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace API.Controllers;

public class PaymentsController(PaymentsService paymentsService, 
  StoreContext context, IConfiguration config, ILogger<PaymentsController>logger)
    : BaseApiController
{
  [Authorize]
  [HttpPost]
  public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent()
  {
    var basket = await context.Baskets.GetBasketWithItemss(Request.Cookies["basketId"]);

    if (basket == null) return BadRequest("Problem with the basket");

    var intent = await paymentsService.CreateOrUpdatePaymentIntent(basket);

    if (intent == null) return BadRequest("Problem creating payment intent");

    basket.PaymentIntentId ??= intent.Id;
    basket.ClientSecret ??= intent.ClientSecret;

    if (context.ChangeTracker.HasChanges())
    {
      var result = await context.SaveChangesAsync()>0;

      if(!result) return BadRequest("Problem updating basket with intent");
    }
    return basket.ToDto();
  }

  [HttpPost("webhook")]
  public async Task<IActionResult>StripeWebhook()
  {
    var json= await new StreamReader(Request.Body).ReadToEndAsync();

    try
    {
      var stripeEvent=ConstructStripeEvent(json);
      Console.WriteLine($"[WEBHOOK] Received stripe event type: {stripeEvent.Type}");

      // Only handle payment_intent events
      if (stripeEvent.Type != "payment_intent.succeeded" && stripeEvent.Type != "payment_intent.payment_failed")
      {
        Console.WriteLine($"[WEBHOOK] Ignoring event type: {stripeEvent.Type}");
        logger.LogInformation($"Ignoring webhook event type: {stripeEvent.Type}");
        return Ok(); // Still return OK to acknowledge receipt
      }

      if (stripeEvent.Data.Object is not PaymentIntent intent)
      {
        Console.WriteLine("[WEBHOOK] Event is not a PaymentIntent");
        logger.LogWarning("Webhook received but not a PaymentIntent");
        return BadRequest("Invalid event data");
      }
      
      Console.WriteLine($"[WEBHOOK] PaymentIntent {intent.Id} status: {intent.Status}");
      logger.LogInformation($"Processing payment intent {intent.Id} with status {intent.Status}");
      
      if (intent.Status =="succeeded") 
      {
        Console.WriteLine($"[WEBHOOK] Calling HandlePaymentIntentSucceeded for {intent.Id}");
        await HandlePaymentIntentSucceeded(intent);
      }
      else 
      {
        Console.WriteLine($"[WEBHOOK] Calling HandlePaymentIntentFailed for {intent.Id}");
        await HandlePaymentIntentFailed(intent);
      }

      Console.WriteLine($"[WEBHOOK] Successfully processed payment intent {intent.Id}");
      logger.LogInformation($"Successfully processed payment intent {intent.Id}");
      return Ok();
    }
    catch (StripeException ex)
    {
      Console.WriteLine($"[WEBHOOK] StripeException: {ex.Message}");
      logger.LogError(ex, "Stripe webhook error");
      return StatusCode(StatusCodes.Status500InternalServerError,"Webhook error");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"[WEBHOOK] Exception: {ex.Message}");
      Console.WriteLine($"[WEBHOOK] Stack trace: {ex.StackTrace}");
      logger.LogError(ex,"An unexpected error has occured");
      return StatusCode(StatusCodes.Status500InternalServerError,"Unexpected error");
    }
  }

  private async Task HandlePaymentIntentFailed(PaymentIntent intent)
  {
    var order = await context.Orders
      .Include(x => x.OrderItems)
      .FirstOrDefaultAsync(x => x.PaymentIntentId == intent.Id)
        ?? throw new Exception ("Order not found");

    foreach (var item in order.OrderItems)
    {
      var productItem = await context.Products
        .FindAsync(item.ItemOrdered.ProductId)
          ?? throw new Exception ("Problem updating order stock");

      productItem.QuantityInStock += item.Quantity;
    }

    order.OrderStatus = OrderStatus.PaymentFailed;

    await context.SaveChangesAsync();
  }

  private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
  {
    Console.WriteLine($"[PAYMENT SUCCESS] Starting for intent {intent.Id}, amount: {intent.Amount}");
    logger.LogInformation($"HandlePaymentIntentSucceeded called for intent {intent.Id}, amount: {intent.Amount}");
    
    // Retry logic for SQLite concurrency issues - order might not be visible yet
    Order? order = null;
    int retryCount = 0;
    const int maxRetries = 5;
    const int delayMs = 500;

    while (order == null && retryCount < maxRetries)
    {
      try
      {
        Console.WriteLine($"[PAYMENT SUCCESS] Attempt {retryCount + 1}/{maxRetries} to find order for intent {intent.Id}");
        
        order = await context.Orders
          .Include(x => x.OrderItems)
          .FirstOrDefaultAsync(x => x.PaymentIntentId == intent.Id);

        if (order == null)
        {
          retryCount++;
          if (retryCount < maxRetries)
          {
            Console.WriteLine($"[PAYMENT SUCCESS] Order not found, waiting {delayMs}ms before retry...");
            logger.LogWarning($"Order not found for payment intent {intent.Id}, retrying... ({retryCount}/{maxRetries})");
            await Task.Delay(delayMs);
          }
        }
        else
        {
          Console.WriteLine($"[PAYMENT SUCCESS] Found order {order.Id}!");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[PAYMENT SUCCESS] Database error on attempt {retryCount + 1}: {ex.Message}");
        logger.LogWarning($"Database query error, retrying: {ex.Message}");
        retryCount++;
        if (retryCount < maxRetries)
        {
          await Task.Delay(delayMs);
        }
      }
    }

    if (order == null)
    {
      Console.WriteLine($"[PAYMENT SUCCESS] FAILED - Order not found after {maxRetries} retries for intent {intent.Id}");
      logger.LogError($"Order not found for payment intent {intent.Id} after {maxRetries} retries");
      throw new Exception("Order not found");
    }

    Console.WriteLine($"[PAYMENT SUCCESS] Comparing amounts - Order total: {order.GetTotal()}, Intent amount: {intent.Amount}");
    logger.LogInformation($"Found order {order.Id} with total {order.GetTotal()}, comparing to intent amount {intent.Amount}");

    if (order.GetTotal() != intent.Amount)
    {
      Console.WriteLine($"[PAYMENT SUCCESS] Amount mismatch detected!");
      logger.LogWarning($"Amount mismatch: Order total {order.GetTotal()} != Intent amount {intent.Amount}");
      order.OrderStatus= OrderStatus.PaymentMismatch;
    }
    else
    {
      Console.WriteLine($"[PAYMENT SUCCESS] Amount matched! Updating order status to PaymentReceived");
      logger.LogInformation($"Amount matched, updating order {order.Id} status to PaymentReceived");
      order.OrderStatus = OrderStatus.PaymentReceived;
    }

    var basket = await context.Baskets.FirstOrDefaultAsync(x=>
      x.PaymentIntentId==intent.Id);

    if (basket !=null) 
    {
      Console.WriteLine($"[PAYMENT SUCCESS] Removing basket {basket.BasketId}");
      logger.LogInformation($"Removing basket {basket.BasketId} after successful payment");
      context.Baskets.Remove(basket);
    }

    Console.WriteLine($"[PAYMENT SUCCESS] Saving changes to database...");
    await context.SaveChangesAsync();
    Console.WriteLine($"[PAYMENT SUCCESS] SUCCESS - Order {order.Id} updated to {order.OrderStatus}");
    logger.LogInformation($"Successfully updated order {order.Id} status to {order.OrderStatus}");
  }
 

  private Event ConstructStripeEvent(string json)
  {
    try
    {
      return EventUtility.ConstructEvent(json,
       Request.Headers["Stripe-Signature"], config["StripeSettings:WhSecret"]);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to construct stripe event");
      throw new StripeException("Invalid signature");
    }
  }
}
