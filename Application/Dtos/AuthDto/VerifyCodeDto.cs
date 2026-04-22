using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.AuthDto;

public class VerifyCodeDto
{
    [EmailAddress(ErrorMessage = "Invalid email format")] 
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Verification code is required.")]
    [StringLength(6, MinimumLength = 4, ErrorMessage = "Code must be between 4 and 6 characters")]
    public string Code { get; set; } = null!;
}