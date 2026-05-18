using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.StudentDto;

public class StudentCreateDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = null!;
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = null!;
    [Phone]
    public string? Phone { get; set; }

    public DateTime? DateOfBirth { get; set; }
}