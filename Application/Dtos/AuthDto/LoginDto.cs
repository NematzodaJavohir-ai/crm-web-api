using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.AuthDto;

public class LoginDto
{
    [EmailAddress(ErrorMessage = "Invalid email format")] 
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 50 characters.")]
    public string Password { get; set; } = null!;
}