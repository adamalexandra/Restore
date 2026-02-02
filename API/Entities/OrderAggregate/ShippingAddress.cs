using System;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.OrderAggregate;

[Owned]
public class ShippingAddress
{
  public required string Name { get; set; }
  public required string Line1 { get; set; }
  public required string City { get; set; }

  [JsonPropertyName("postal_code")]
  public required string PostalCode { get; set; }
  public required string Country { get; set; }
}


