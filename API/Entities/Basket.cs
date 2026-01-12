using System;

namespace API.Entities;

public class Basket
{
  public int Id { get; set; }
  public required string BasketId { get; set; }
  public List<BasketItem> Items { get; set;}=[];
  public string? ClientSecret { get; set;}
  public string? PaymentIntentId { get; set;}
  public void AddItem(Product product, int quantity)
  {
    if (product == null) ArgumentNullException.ThrowIfNull(product);
    if (quantity<=0) throw new ArgumentException("Quantity should be greater than zero", 
      nameof(quantity));

    var existingItem = FindItem(product.Id);

    if (existingItem == null)
    {
      Items.Add(new BasketItem
      {
        Product = product,
        Quantity = quantity,
        
      });
    }
    else
    {
      existingItem.Quantity += quantity; //increse the quantity of the item in the basket
    }
  }

  public void RemoveItem(int productId, int quantity)
  {
    if (quantity <=0) throw new ArgumentException("Quantity should be greater than zero",
      nameof(quantity));

    var item = FindItem(productId);
    if (item == null) return;

    item.Quantity-= quantity; //decrese the quantity of the item in the basket
    if (item.Quantity <= 0) Items.Remove (item);
  }
  private BasketItem? FindItem (int productId)
  {
    return Items.FirstOrDefault(item => item.ProductId == productId); //return first item that matches if exists in our list or default 
  }
}
