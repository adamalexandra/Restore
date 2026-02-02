using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Entities.OrderAggregate;
using API.Extensions;
using API.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Authorize]
public class OrdersController(StoreContext context) : BaseApiController
{
  [HttpGet]
  public async Task<ActionResult<List<OrderDto>>> GetOrders()
  {
    var orders = await context.Orders
    .ProjectToDto()
    .Where(x => x.BuyerEmail == User.GetUsername())
    .ToListAsync();

    return orders;
  }

  [HttpGet("{id:int}")]

  public async Task<ActionResult<OrderDto>> GetOrderDetails(int id)
  {
    var order = await context.Orders
      .ProjectToDto()
      .Where(x => x.BuyerEmail == User.GetUsername() && id == x.Id)
      .FirstOrDefaultAsync();
    
    if (order == null) return NotFound();

    return order;
  }

  [HttpPost]
  public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
  {
    var basket = await context.Baskets.GetBasketWithItemss(Request.Cookies["basketId"]);

    if (basket == null || basket.Items.Count == 0 || string.IsNullOrEmpty(basket.PaymentIntentId))
    {
      Console.WriteLine($"[CREATE_ORDER] Bad request - Basket: {(basket == null ? "null" : "exists")}, Items: {basket?.Items.Count ?? 0}, PaymentIntentId: {basket?.PaymentIntentId ?? "null"}");
      return BadRequest("Basket is empty or not found");
    }
    
    Console.WriteLine($"[CREATE_ORDER] Creating order with PaymentIntentId: {basket.PaymentIntentId}");
    
    var items = CreateOrderItems(basket.Items);
    if (items == null) return BadRequest("Some items out of stock");

    var subtotal = items.Sum(x => x.Price * x.Quantity);
    var deliveryFee = CalculateDeliveryFee(subtotal);

    var order = await context.Orders
      .Include(x=> x.OrderItems)
      .FirstOrDefaultAsync(x=> x.PaymentIntentId==basket.PaymentIntentId);

    if (order==null)
    {
     order = new Order
    {
      OrderItems = items,
      BuyerEmail = User.GetUsername(),
      ShippingAddress = orderDto.ShippingAddress,
      DeliveryFee = deliveryFee,
      Subtotal = subtotal,
      PaymentSummary = orderDto.PaymentSummary,
      PaymentIntentId = basket.PaymentIntentId
    }; 

    Console.WriteLine($"[CREATE_ORDER] New order created - Id will be assigned after save, PaymentIntentId: {order.PaymentIntentId}");
    context.Orders.Add(order);
    }
    else
    {
      Console.WriteLine($"[CREATE_ORDER] Order already exists with Id: {order.Id}");
      order.OrderItems=items;
    }

    var result = await context.SaveChangesAsync() > 0;

    if (!result)
    {
      Console.WriteLine("[CREATE_ORDER] SaveChangesAsync returned false");
      return BadRequest("Problem creating order");
    }

    Console.WriteLine($"[CREATE_ORDER] SUCCESS - Order {order.Id} created with PaymentIntentId: {order.PaymentIntentId}, Status: {order.OrderStatus}");
    return CreatedAtAction(nameof(GetOrderDetails), new {id = order.Id}, order.ToDto());
  }

  private long CalculateDeliveryFee(long subtotal)
  {
    return subtotal > 10000 ? 0 :300;
  }

  private List<OrderItem>? CreateOrderItems(List<BasketItem> items)
  {
    var orderItems = new List<OrderItem>();

    foreach (var item in items)
    {
      if (item.Product.QuantityInStock < item.Quantity)
        return null;

      var orderItem = new OrderItem
      {
        ItemOrdered = new ProductItemOrdered
        {
          ProductId = item.ProductId,
          PictureUrl = item.Product.PictureUrl,
          Name = item.Product.Name
        },
        Price = item.Product.Price,
        Quantity = item.Quantity
      };
      orderItems.Add(orderItem);

      item.Product.QuantityInStock -= item.Quantity;
    }

    return orderItems;
  }
}
