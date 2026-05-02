using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.StudentDto;

public class StudentCreateDto
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; } = null!;

    [Url(ErrorMessage = "Photo path must be a valid URL")]
    public string? PhotoUrl { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? Phone { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [RegularExpression(@"^@?[\w\d_]{5,32}$", ErrorMessage = "Invalid Telegram username")]
    public string? TelegramUsername { get; set; }

    [Url(ErrorMessage = "Invalid GitHub URL")]
    public string? GithubUrl { get; set; }

    [MaxLength(1000, ErrorMessage = "About me section cannot exceed 1000 characters")]
    public string? AboutMe { get; set; }
}