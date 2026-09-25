namespace API.DTOs;

public class ShippingAddressDto
{
  public string? Name { get; set; }
  public string Line1 { get; set; } = string.Empty;
  public string PostalCode { get; set; } = string.Empty;
  public string City { get; set; } = string.Empty;
  public string Country { get; set; } = string.Empty;
}
