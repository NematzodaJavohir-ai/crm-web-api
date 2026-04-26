using System;
using Application.Dtos.GroupDto;
using Application.Dtos.WeeklyResultDto;

namespace Application.Dtos.StudentDto;

public class StudentFullProfileDto
{
     public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? PhotoUrl { get; set; }
    public string? TelegramUsername { get; set; }
    public string? GithubUrl { get; set; }
    public string? AboutMe { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime EnrollDate { get; set; }
    public IEnumerable<GroupShortDto> Groups { get; set; } = new List<GroupShortDto>();
    public IEnumerable<WeeklyResultResponseDto> WeekResults { get; set; } = new List<WeeklyResultResponseDto>();
    public double AverageScore { get; set; }
    public int TotalAbsences { get; set; }
}
