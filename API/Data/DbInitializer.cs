using System;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.WebEncoders.Testing;
using API.Entities;
namespace API.Data;

public class DbInitializer
{
  public static void InitDb(WebApplication app)
  {
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<StoreContext>()
      ?? throw new InvalidOperationException("Failed to retrieve store context");
    SeedData(context);
  }

  private static void SeedData(StoreContext context)
  {
    context.Database.Migrate();

    if (context.Products.Any()) return;

    var products = new List<Product>
    {
      new Product
      {
        Name = "Cinnamon Roll",
        Description =
                        "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
        Price = 13.99,
        PictureUrl = "/images/products/sb-ang1.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 18
      },
      new Product
      {
        Name = "Cookie Dough",
        Description = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.",
        Price = 13.99,
        PictureUrl = "/images/products/sb-ang2.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 23
      },
      new Product
      {
        Name = "Flower Garden",
        Description =
                        "Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. Mauris eget neque at sem venenatis eleifend. Ut nonummy.",
        Price = 15.99,
        PictureUrl = "/images/products/sb-core1.png",
        Brand = "HouseOfCandles",
        Type = "Floral",
        QuantityInStock = 32
      },
      new Product
      {
        Name = "Jasmine",
        Description =
                        "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.",
        Price = 12.99,
        PictureUrl = "/images/products/sb-core2.png",
        Brand = "HouseOfCandles",
        Type = "Floral",
        QuantityInStock = 12
      },
      new Product
      {
        Name = "Black Orchid",
        Description =
                        "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
        Price = 19.99,
        PictureUrl = "/images/products/sb-react1.png",
        Brand = "HouseOfCandles",
        Type = "Floral",
        QuantityInStock = 11
      },
      new Product
      {
        Name = "Coconut",
        Description =
                        "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
        Price = 6.99,
        PictureUrl = "/images/products/sb-ts1.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 25
      },
      new Product
      {
        Name = "Salted Caramel",
        Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
        Price = 16.99,
        PictureUrl = "/images/products/hat-core1.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 100
      },
      new Product
      {
        Name = "Lavender",
        Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
        Price = 11.99,
        PictureUrl = "/images/products/hat-react1.png",
        Brand = "HouseOfCandles",
        Type = "Floral",
        QuantityInStock = 16
      },
      new Product
      {
        Name = "Ocean Dive",
        Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
        Price = 9.99,
        PictureUrl = "/images/products/hat-react2.png",
        Brand = "CandleLand",
        Type = "Fresh",
        QuantityInStock = 38
      },
      new Product
      {
        Name = "Cotton Candy",
        Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
        Price = 1800,
        PictureUrl = "/images/products/glove-code1.png",
        Brand = "HouseOfCandles",
        Type = "Fresh",
        QuantityInStock = 3
      },
      new Product
      {
        Name = "Apple Pie",
        Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
        Price = 15.99,
        PictureUrl = "/images/products/glove-code2.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 14
      },
      new Product
      {
        Name = "Rose Petals",
        Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
        Price = 1600,
        PictureUrl = "/images/products/glove-react1.png",
        Brand = "HouseOfCandles",
        Type = "Floral",
        QuantityInStock = 15
      },
      new Product
      {
        Name = "Croissant",
        Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
        Price = 9.99,
        PictureUrl = "/images/products/glove-react2.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 17
      },
      new Product
      {
        Name = "Fresh Laundry",
        Description =
                        "Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. Mauris eget neque at sem venenatis eleifend. Ut nonummy.",
        Price = 10.99,
        PictureUrl = "/images/products/boot-redis1.png",
        Brand = "CandleLand",
        Type = "Fresh",
        QuantityInStock = 22
      },
      new Product
      {
        Name = "Candle Pot",
        Description =
                        "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
        Price = 42.99,
        PictureUrl = "/images/products/boot-core2.png",
        Brand = "CandleLab",
        Type = "Diy",
        QuantityInStock = 4
      },
      new Product
      {
        Name = "Wood Wisk",
        Description =
                        "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.",
        Price = 5.99,
        PictureUrl = "/images/products/boot-core1.png",
        Brand = "CandleLab",
        Type = "Diy",
        QuantityInStock = 45
      },
      new Product
      {
        Name = "Candle Wax",
        Description = "Aenean nec lorem. In porttitor. Donec laoreet nonummy augue.",
        Price = 32.99,
        PictureUrl = "/images/products/boot-ang2.png",
        Brand = "CandleLand",
        Type = "Diy",
        QuantityInStock = 14
      },
      new Product
      {
        Name = "Essential Oil",
        Description =
                        "Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. Mauris eget neque at sem venenatis eleifend. Ut nonummy.",
        Price = 9.99,
        PictureUrl = "/images/products/boot-ang1.png",
        Brand = "CandleLab",
        Type = "Diy",
        QuantityInStock = 11
      }

    };
    context.Products.AddRange(products);
    context.SaveChanges();
  }
}
