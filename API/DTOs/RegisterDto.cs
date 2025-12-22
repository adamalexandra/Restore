using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
  [Required]
  public required string Email { get; set; } = string.Empty;
  public required string Password { get; set; } 
}
