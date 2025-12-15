using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extentions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class BasketController(StoreContext context) : BaseApiController
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
}
