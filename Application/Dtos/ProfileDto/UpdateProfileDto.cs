using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.ProfileDto;

public class UpdateProfileDto
{
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    public string? FirstName { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    public string? LastName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string? Phone { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(300, ErrorMessage = "Address cannot exceed 300 characters")]
    public string? Address { get; set; }

    [MaxLength(50, ErrorMessage = "Telegram username cannot exceed 50 characters")]
    [RegularExpression(@"^@?[\w\d_]{5,32}$", ErrorMessage = "Invalid Telegram username")]
    public string? TelegramUsername { get; set; }

    [Url(ErrorMessage = "Invalid LinkedIn URL")]
    [MaxLength(300, ErrorMessage = "LinkedIn URL cannot exceed 300 characters")]
    public string? LinkedInUrl { get; set; }

    [Url(ErrorMessage = "Invalid GitHub URL")]
    [MaxLength(300, ErrorMessage = "GitHub URL cannot exceed 300 characters")]
    public string? GithubUrl { get; set; }

    [MaxLength(1000, ErrorMessage = "About me cannot exceed 1000 characters")]
    public string? AboutMe { get; set; }
}