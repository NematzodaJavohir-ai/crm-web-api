using Application.Dtos.CourseDto;
using Application.Dtos.GroupDto;
using Domain.Enums;

namespace Application.Dtos.MentorDto;

public class MentorFullProfileDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Specialization { get; set; }
    public int? ExperienceYears { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GithubUrl { get; set; }
    public bool IsActive { get; set; }

    // ───── Invite Status ─────
    public bool IsPasswordSet { get; set; }
    public InviteStatus InviteStatus { get; set; }

    public DateTime HireDate { get; set; }
    public DateTime CreatedAt { get; set; }

    // ───── Stats ─────
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int TotalGroups { get; set; }
    public int ActiveGroups { get; set; }
    public int CompletedGroups { get; set; }
    public int TotalLessons { get; set; }
    public int TotalCourses { get; set; }
    public double AverageGroupScore { get; set; }   

    // ───── Re.Data ─────
    public List<GroupShortDto> Groups { get; set; } = new();
    public List<CourseShortDto> Courses { get; set; } = new();
}