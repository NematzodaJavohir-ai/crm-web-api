using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.UserDto;

public class CreateUserDto
{
    [Required(ErrorMessage = "FirstName is required!")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "LastName is required!")]
    public string LastName { get; set; } = null!;

    [EmailAddress, Required(ErrorMessage = "Email is required!")]
    public string Email { get; set; } = null!;
     
    [Required(ErrorMessage = "PhoneNumber is Required!")]
    [MaxLength(20), MinLength(5)]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Password is Required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters long!")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "ConfirmPassword is Required")]
    [Compare("Password", ErrorMessage = "Passwords do not match!")]
    public string ConfirmPassword { get; set; } = null!;
}