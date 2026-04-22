using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.UserDto;

public class UpdateUserDto
{
    [Required(ErrorMessage = "FirstName is required!")]
    [MaxLength(50), MinLength(2)]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "LastName is required!")]
    [MaxLength(50), MinLength(2)]
    public string LastName { get; set; } = null!;

    [EmailAddress, Required(ErrorMessage = "Email is required!")]
    public string Email { get; set; } = null!;
     
    [Required(ErrorMessage = "PhoneNumber is Required!")]
    [MaxLength(20), MinLength(5)]
    public string PhoneNumber { get; set; } = null!;
}