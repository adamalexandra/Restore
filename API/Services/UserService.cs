using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UserService(StoreContext context) : IUserService
{
  public async Task<UserProfileDto?> GetProfileAsync(string userId)
  {
    var user = await context.Users
      .Include(u => u.Address)
      .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null) return null;

    return new UserProfileDto
    {
      Email = user.Email ?? string.Empty,
      Name = user.Name ?? user.UserName ?? string.Empty,

      Address = user.Address == null ? null : new ShippingAddressDto
      {
        Name = user.Address.Name,
        Line1 = user.Address.Line1,
        City = user.Address.City,
        PostalCode = user.Address.PostalCode,
        Country = user.Address.Country
      }
    };
  }

  public async Task<bool> UpdateProfileAsync(string userId, UpdateUserProfileDto dto)
  {
    var user = await context.Users
      .Include(u => u.Address)
      .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null) return false;

    if (dto.Name != null) user.Name = dto.Name;

    if (dto.Address != null)
    {
      var addressName = dto.Address.Name ?? dto.Name ?? user.Name ?? string.Empty;

      if (user.Address == null)
      {
        user.Address = new Address
        {
          Name = addressName,
          Line1 = dto.Address.Line1,
          City = dto.Address.City,
          PostalCode = dto.Address.PostalCode,
          Country = dto.Address.Country
        };
      }
      else
      {
        user.Address.Name = addressName;
        user.Address.Line1 = dto.Address.Line1;
        user.Address.City = dto.Address.City;
        user.Address.PostalCode = dto.Address.PostalCode;
        user.Address.Country = dto.Address.Country;
      }
    }

    await context.SaveChangesAsync();
    return true;
  }
}
