using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.MentorDto;

public class MentorUpdateDto
{
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? Phone { get; set; }

    [StringLength(100, ErrorMessage = "Specialization is too long")]
    public string? Specialization { get; set; }

    [Range(0, 50, ErrorMessage = "Experience years must be between 0 and 50")]
    public int? ExperienceYears { get; set; }

    [MaxLength(2000, ErrorMessage = "Bio cannot exceed 2000 characters")]
    public string? Bio { get; set; }

    [Url(ErrorMessage = "Invalid LinkedIn URL")]
    public string? LinkedInUrl { get; set; }

    [Url(ErrorMessage = "Invalid GitHub URL")]
    public string? GithubUrl { get; set; }
}