namespace API.DTOs;

public class UpdateUserProfileDto
{
  public string? Name {get; set;}
  public ShippingAddressDto? Address {get; set;}
}
