using API.DTOs;

namespace API.Services;

public interface IUserService
{
  Task<UserProfileDto?> GetProfileAsync(string userId);
  Task<bool> UpdateProfileAsync(string userId, UpdateUserProfileDto dto);
}
