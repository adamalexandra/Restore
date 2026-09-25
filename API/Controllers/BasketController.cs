using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiscountService = API.Services.DiscountService;

namespace API.Controllers;

public class BasketController(StoreContext context,
  DiscountService couponService, PaymentsService paymentsService) : BaseApiController
{
  [HttpGet]
  public async Task<ActionResult<BasketDto>>GetBasket()
  {
    var basket = await RetrieveBasket();

    if (basket == null) return NoContent();

    return basket.ToDto();
  }

  [HttpPost]
  public async Task<ActionResult> AddItemToBasket(int productId, int quantity)
  {
    var basket = await RetrieveBasket(); //if user has already a basket

    basket ??= CreateBasket(); //if (basket == null) basket = CreateBasket()
    
    var product = await context.Products.FindAsync(productId); //find the product to add

    if (product == null) return BadRequest("Problem adding item to basket"); //product does not exist
    
    basket.AddItem(product, quantity); // add product to the basket

    var result = await context.SaveChangesAsync() > 0 ; //saves to database, if one change fails all fail

    if (result == true) return CreatedAtAction(nameof(GetBasket), basket.ToDto());

    return BadRequest("Problem updating basket");
  }


  [HttpDelete]
  public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
  {
    var basket = await RetrieveBasket();

    if (basket == null) return BadRequest("Unable to retrieve basket");

    basket.RemoveItem(productId, quantity);

    var result = await context.SaveChangesAsync() > 0 ;

    if (result) return Ok();

    return BadRequest("Problem updating basket");
  }

  private Basket CreateBasket()
  {
    var basketId = Guid.NewGuid().ToString(); //create unique id for the basket
    var cookieOptions = new CookieOptions
    {
      IsEssential = true, //mark the cookie as essential (even if a user denies)
      Expires = DateTime.UtcNow.AddDays(30) //for 30 days
    };
    Response.Cookies.Append("basketId", basketId, cookieOptions); //send cookie to the client
    var basket = new Basket {BasketId =basketId}; //new basket
    context.Baskets.Add(basket); //dose not yet write anything to the database 
    return basket;
  }
    private async Task<Basket?> RetrieveBasket()
  {
    return await context.Baskets
    .Include(x => x.Items) // (item => item.Items)
    .ThenInclude(x => x.Product)
    .FirstOrDefaultAsync(x => x.BasketId == Request.Cookies["basketId"]);
  }


[HttpPost("{code}")]
public async Task<ActionResult<BasketDto>> AddCouponCode(string code)
  {
    //get the basket
    var basket = await RetrieveBasket();
    if (basket == null)// || string.IsNullOrEmpty(basket.ClientSecret))
      return BadRequest("Unable to apply voucher");

    //get the coupon
    var coupon = await couponService.GetCouponFromPromoCode(code);
    if (coupon == null) return BadRequest("Invalid coupon");
    

    //update the basket with the coupon
    basket.Coupon = coupon;

    //update the payment intent if exists
    
   // var intent = await paymentsService.CreateOrUpdatePaymentIntent(basket);
   // if (intent == null) return BadRequest("Problem applying coupon to basket");
  if (!string.IsNullOrEmpty(basket.ClientSecret))
    {
      var intent = await paymentsService.CreateOrUpdatePaymentIntent(basket);
      if (intent == null) return BadRequest("Problem applying coupon to basket");
    }

    // save changes and return BasketDto if successful
    var result = await context.SaveChangesAsync() > 0;

    if (result) return CreatedAtAction(nameof(GetBasket), basket.ToDto());
    return BadRequest("Problem updating basket");
  }

  [HttpDelete("remove-coupon")]
  public async Task<ActionResult> RemoveCouponFromBasket()
  {
    //get thebasket
    var basket = await RetrieveBasket();
    if (basket == null || basket.Coupon == null)
      return BadRequest("Unable to update basket with coupon");

    if (!string.IsNullOrEmpty(basket.ClientSecret))
    {
      var intent = await paymentsService.CreateOrUpdatePaymentIntent(basket, true);
      if (intent == null) return BadRequest("Problem removing coupon from basket");
    }

    basket.Coupon = null;
    
    var result = await context.SaveChangesAsync() > 0;
    
    if (result) return Ok();
    
    return BadRequest("Problem updating basket");
  }
}