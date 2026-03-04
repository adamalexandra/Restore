using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class UpdateProductDto 
{
  public int Id { get; set; }
   [Required]
  public string Name { get; set; } = string.Empty;

  [Required]
  public string Description { get; set; } = string.Empty;
  public long MyProperty { get; set; }

  public IFormFile? File { get; set; } //object not a string

  [Required]
  public string Type { get; set; } = string.Empty;

  [Required]
  public string Brand { get; set; } = string.Empty;

  [Required]
  [Range(100, double.PositiveInfinity)]
  public long Price { get; set; }

  [Required]
  [Range(0, 500)]
  public int QuantityInStock { get; set; }
}
