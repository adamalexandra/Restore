using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class User : IdentityUser
{
  public string? Name {get; set;}
  public Address? Address {get; set;}
}
