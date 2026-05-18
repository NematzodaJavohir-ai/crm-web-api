using Application.Dtos.GroupDto;

namespace Application.Dtos.StudentDto;

public class StudentFullProfileDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? TelegramUsername { get; set; }
    public string? GithubUrl { get; set; }
    public string? AboutMe { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public DateTime EnrollDate { get; set; }

    // Stats
    public int TotalGroups { get; set; }
    public int ActiveGroups { get; set; }
    public int CompletedGroups { get; set; }
    public double AttendanceRate { get; set; }
    public double AverageScore { get; set; }
    public decimal TotalPaid { get; set; }
    public bool IsRecommendedForCertificate { get; set; }

    // Related
    public IEnumerable<GroupShortDto> Groups { get; set; } = new List<GroupShortDto>();
}