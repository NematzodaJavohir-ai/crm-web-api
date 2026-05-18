using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.MentorDto;

public class MentorUpdateDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Specialization { get; set; }
    public int? ExperienceYears { get; set; }
    public string? Bio { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GithubUrl { get; set; }
    public string? PhotoUrl { get; set; }
}