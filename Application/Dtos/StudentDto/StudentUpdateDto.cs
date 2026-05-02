using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.StudentDto;

public class StudentUpdateDto
{
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