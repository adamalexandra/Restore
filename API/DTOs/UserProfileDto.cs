namespace API.DTOs;

public class UserProfileDto
{
  public required string Name {get; set;}
  public required string Email {get; set;}
  public ShippingAddressDto? Address {get; set;}
}
