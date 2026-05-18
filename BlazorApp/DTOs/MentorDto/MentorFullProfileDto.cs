using System;
using BlazorApp.DTOs.CourseDto;
using BlazorApp.DTOs.GroupDto;

namespace BlazorApp.DTOs.MentorDto;

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
    public DateTime HireDate { get; set; }
    
    // Дополнительные поля для полного профиля
    public double AverageRating { get; set; }
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
    public int TotalGroups { get; set; }
    
    // Связанные данные
    public List<GroupShortDto> Groups { get; set; } = new();
    public List<CourseShortDto> Courses { get; set; } = new();
}

