using System;
using BlazorApp.DTOs.GroupDto;
using BlazorApp.DTOs.WeeklyResultDto;


namespace BlazorApp.DTOs.StudentDto;

public class StudentFullProfileDto
{
     public int Id { get; set; }
    public int UserId { get; set; }
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
