using System;
using API.Entities;
using API.Entities.OrderAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;

namespace API.Data;

public class StoreContext (DbContextOptions options) : IdentityDbContext<User> (options)
{
  public required DbSet<Product> Products { get; set; }
  public required DbSet<Basket> Baskets { get; set; }
  public required DbSet<Order> Orders {get; set;}

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<IdentityRole>()
      .HasData(
        new IdentityRole {Id = "4bcfc975-47dd-469a-85ef-4d2a5626e4ec", Name="Member", NormalizedName="MEMBER"},
        new IdentityRole {Id = "1c9fe65c-3a87-4947-b304-26d7e7f4fed5", Name="Admin", NormalizedName="ADMIN"}
      );
  }
}
