using System;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class User : IdentityUser
{
  public int AddressIde {get; set;}
  public Address? Address {get; set;}
}
