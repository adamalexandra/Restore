using API.DTOs;
using API.Entities;

namespace API.Extensions;

public static class ProductExtensions
{
  public static IQueryable<Product> Sort(this IQueryable<Product> query, string? orderBy)
  {
    query = orderBy switch
    {
      "price" => query.OrderBy(x => x.Price),
      "priceDesc"=> query.OrderByDescending(x => x.Price),
      _=> query.OrderBy(x => x.Name)
    }; //does not do anything with the database
  return query;
  }
  public static IQueryable<Product> Search(this IQueryable<Product> query, string? searchTerm)
  {
    if (string.IsNullOrEmpty(searchTerm)) return query;

    var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

    return query.Where(x => x.Name.ToLower().Contains(lowerCaseSearchTerm));
  }

  public static IQueryable<Product>Filter(this IQueryable<Product> query,
  string? brands, string? types)
  {
    var brandList = new List<string> ();
    var typeList = new List <string> ();
    if (!string.IsNullOrEmpty(brands))
    {
      brandList.AddRange([.. brands.ToLower().Split(",")]);
    }

    if (!string.IsNullOrEmpty(types))
    {
      typeList.AddRange([.. types.ToLower().Split(",")]);
    }

    query = query.Where(x => brandList.Count == 0 || brandList.Contains (x.Brand.ToLower()));
    query = query.Where(x => typeList.Count == 0 || typeList.Contains (x.Type.ToLower()));

    return query;
  }

  public static Product ToEntity(this CreateProductDto dto)
  {
    return new Product
    {
      Name = dto.Name,
      Description = dto.Description,
      Type = dto.Type,
      Brand = dto.Brand,
      Price = dto.Price,
      QuantityInStock = dto.QuantityInStock,
      PictureUrl = string.Empty //replaced by the uploaded image below
    };
  }

  public static void ApplyTo(this UpdateProductDto dto, Product product)
  {
    product.Name = dto.Name;
    product.Description = dto.Description;
    product.Type = dto.Type;
    product.Brand = dto.Brand;
    product.Price = dto.Price;
    product.QuantityInStock = dto.QuantityInStock;
  }
}
