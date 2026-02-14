using System;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.WebEncoders.Testing;
using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Data;

public class DbInitializer
{
  public static async Task InitDb(WebApplication app)
  {
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<StoreContext>()
      ?? throw new InvalidOperationException("Failed to retrieve store context");
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>()
      ?? throw new InvalidOperationException("Failed to retrieve user manager");
    
     SeedDataAsync(context, userManager).GetAwaiter().GetResult(); //instead of await
  }

  private static void SeedData(StoreContext context, object userManger)
  {
    throw new NotImplementedException();
  }

  private static async Task SeedDataAsync(StoreContext context, UserManager<User>userManager)
  {
    context.Database.Migrate();

    if(!userManager.Users.Any())
    {
      var user = new User
      {
        UserName = "bob@test.com",
        Email = "bob@test.com"
      };
      await userManager.CreateAsync(user, "Pa$$w0rd");
      await userManager.AddToRoleAsync(user, "Member");

      var admin = new User
      {
        UserName = "admin@test.com",
        Email = "admin@test.com"
      };
      await userManager.CreateAsync(admin, "Pa$$w0rd");
      await userManager.AddToRolesAsync(admin, ["Member", "Admin"]);
    }
    if (context.Products.Any()) return;


    var products = new List<Product>
    {
      new Product
      {
        Name = "Cinnamon Roll",
        Description = "The overall effect is like stepping into a kitchen where cinnamon rolls are baking—comforting, festive, and a little indulgent. It is the kind of scent that makes a space feel welcoming, nostalgic, and perfect for autumn or winter evenings.",
        Price = 1399,
        PictureUrl = "/images-candles/product-candles/cinnamonroll.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 98
      },

      new Product
      {
        Name = "Cookie Dough",
        Description = "The aroma is soft, cozy, and nostalgic—like biting into a warm cookie straight from the oven. It creates a welcoming atmosphere that feels both playful and comforting, perfect for relaxing evenings or adding a touch of sweetness to any space.",
        Price = 1399,
        PictureUrl = "/images-candles/product-candles/cookiedough.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 83
      },

      new Product
      {
        Name = "Flower Garden",
        Description = "The aroma is gentle yet vibrant, evoking spring mornings, blooming gardens, and the serenity of nature. It’s the kind of scent that makes a space feel peaceful, graceful, and full of life.",
        Price = 1599,
        PictureUrl = "/images-candles/product-candles/flowergarden.png",
        Brand = "HouseOfCandles",
        Type = "Floral",
        QuantityInStock = 92
      },

      new Product
      {
        Name = "Jasmine",
        Description ="The overall effect is romantic, soothing, and luxurious—like walking through a moonlit garden where jasmine vines are in full bloom. It creates an atmosphere of tranquility and sensuality, perfect for unwinding or setting a dreamy mood.",
        Price = 1299,
        PictureUrl = "/images-candles/product-candles/jasmine.png",
        Brand = "HouseOfCandles",
        Type = "Floral",
        QuantityInStock = 92
      },

      new Product
      {
        Name = "Black Orchid",
        Description = "The overall effect is luxurious yet calming—like stepping into a serene spa or a lush tropical garden. It creates an atmosphere of grace, tranquility, and quiet sophistication, perfect for moments of relaxation or adding a touch of elegance to your space.",
        Price = 1999,
        PictureUrl = "/images-candles/product-candles/blackorchid.png",
        Brand = "HouseOfCandles",
        Type = "Floral",
        QuantityInStock = 91
      },

      new Product
      {
        Name = "Coconut",
        Description = "The overall effect is relaxing, beachy, and indulgent—like sipping a piña colada under palm trees. It creates an atmosphere of vacation serenity and cozy warmth, perfect for unwinding or bringing a touch of summer into any space.",
        Price = 699,
        PictureUrl = "/images-candles/product-candles/coconut.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 95
      },

      new Product
      {
        Name = "Salted Caramel",
        Description = "The aroma is rich, buttery, and decadently sweet, instantly evoking the sensation of golden caramel melting in a pan. It balances the deep, toasted sugar notes with a creamy smoothness, creating a scent that feels both comforting and luxurious. The subtle hint of salt adds a sophisticated twist, enhancing the sweetness and making the fragrance irresistibly mouthwatering.",
        Price = 1699,
        PictureUrl = "/images-candles/product-candles/saltedcaramel.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 100
      },

      new Product
      {
        Name = "Lavender",
        Description = "The aroma is fresh, herbal, and gently floral, with a soothing sweetness that instantly relaxes the senses. Lavender carries a natural balance of crisp green notes and soft, powdery florals, making it both refreshing and comforting.",
        Price = 1199,
        PictureUrl = "/images-candles/product-candles/lavender.png",
        Brand = "HouseOfCandles",
        Type = "Floral",
        QuantityInStock = 96
      },

      new Product
      {
        Name = "Ocean Dive",
        Description = "The fragrance is crisp, airy, and lightly salty, evoking the feeling of waves crashing against the shore and cool breezes drifting over sand. It blends aquatic notes with subtle hints of citrus and driftwood, creating a clean yet grounding aroma.",
        Price = 999,
        PictureUrl = "/images-candles/product-candles/oceandive.png",
        Brand = "CandleLand",
        Type = "Fresh",
        QuantityInStock = 98
      },

      new Product
      {
        Name = "Cotton Candy",
        Description = "The fragrance is light, airy, and sugary, instantly evoking the joy of carnivals and childhood fairs. It blends the fluffy sweetness of spun sugar with subtle fruity undertones, creating a playful and whimsical aroma.",
        Price = 1800,
        PictureUrl = "/images-candles/product-candles/cottoncandy.png",
        Brand = "HouseOfCandles",
        Type = "Fresh",
        QuantityInStock = 93
      },

      new Product
      {
        Name = "Apple Pie",
        Description ="The aroma is warm, sweet, and spiced, instantly transporting you to a kitchen where a golden pie is cooling on the counter. It blends the juicy freshness of baked apples with the comforting richness of buttery crust and the inviting warmth of cinnamon and nutmeg.",
        Price = 1599,
        PictureUrl = "/images-candles/product-candles/applepie.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 94
      },

      new Product
      {
        Name = "Rose Petals",
        Description ="The fragrance is soft, floral, and delicately sweet, capturing the essence of freshly bloomed roses. It balances the bright, dewy freshness of petals with a velvety depth, creating a scent that feels both uplifting and soothing.",
        Price = 1600,
        PictureUrl = "/images-candles/product-candles/rosepetals.png",
        Brand = "HouseOfCandles",
        Type = "Floral",
        QuantityInStock = 95
      },

      new Product
      {
        Name = "Croissant",
        Description ="The aroma is buttery, warm, and delicately sweet, evoking the moment you bite into a golden, flaky pastry fresh from the oven. It combines the richness of melted butter and toasted dough with subtle hints of vanilla and almond, creating a scent that feels indulgent yet comforting.",
        Price = 1199,
        PictureUrl = "/images-candles/product-candles/croissant.png",
        Brand = "CandleLand",
        Type = "Tasty",
        QuantityInStock = 97
      },

      new Product
      {
        Name = "Fresh Laundry",
        Description ="The aroma is light, airy, and subtly soapy, evoking the feeling of warm clothes just pulled from the dryer or linens drying in the sun. It blends powdery floral notes with a touch of citrus brightness and soft musk, creating a scent that feels both refreshing and cozy.",
        Price = 799,
        PictureUrl = "/images-candles/product-candles/freshlaundry.png",
        Brand = "CandleLand",
        Type = "Fresh",
        QuantityInStock = 92
      },

      new Product
      {
        Name = "Candle Pot",
        Description = "The candle pot is sturdy and durable, designed to safely hold the candle wax and wick while providing an elegant aesthetic.",
        Price = 999,
        PictureUrl = "/images-candles/product-candles/candlepot.png",
        Brand = "CandleLab",
        Type = "DIY",
        QuantityInStock = 94
      },

      new Product
      {
        Name = "Wood Wisk",
        Description = "A wooden wick adds a unique touch to candles, providing a gentle crackling sound when lit, reminiscent of a cozy fireplace. It enhances the ambiance and creates a warm, inviting atmosphere.",
        Price = 599,
        PictureUrl = "/images-candles/product-candles/woodwisk.png",
        Brand = "CandleLab",
        Type = "DIY",
        QuantityInStock = 95
      },

      new Product
      {
        Name = "Candle Wax",
        Description = "High-quality candle wax that ensures a clean burn and enhances the overall candle experience.",
        Price = 3299,
        PictureUrl = "/images-candles/product-candles/candlewax.png",
        Brand = "CandleLand",
        Type = "DIY",
        QuantityInStock = 94
      },

      new Product
      {
        Name = "Essential Oil",
        Description ="Pure essential oils that provide natural fragrance options for candle making, allowing for a personalized scent experience.",
        Price = 999,
        PictureUrl = "/images-candles/product-candles/essentialoil.png",
        Brand = "CandleLab",
        Type = "DIY",
        QuantityInStock = 91
      }

    };
    context.Products.AddRange(products);
    context.SaveChanges();
  }
}
