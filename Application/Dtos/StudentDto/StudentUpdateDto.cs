using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.StudentDto;

public class StudentUpdateDto
{
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    public string? FirstName { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    public string? LastName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string? Phone { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [Url(ErrorMessage = "PhotoUrl must be a valid URL")]
    [MaxLength(500, ErrorMessage = "PhotoUrl cannot exceed 500 characters")]
    public string? PhotoUrl { get; set; }

    [MaxLength(50, ErrorMessage = "Telegram username cannot exceed 50 characters")]
    [RegularExpression(@"^@?[\w\d_]{5,32}$", ErrorMessage = "Invalid Telegram username")]
     public string? TelegramUsername { get; set; }

    [Url(ErrorMessage = "GithubUrl must be a valid URL")]
    [MaxLength(300, ErrorMessage = "GithubUrl cannot exceed 300 characters")]
    public string? GithubUrl { get; set; }

    [MaxLength(1000, ErrorMessage = "About me cannot exceed 1000 characters")]
    public string? AboutMe { get; set; }
}