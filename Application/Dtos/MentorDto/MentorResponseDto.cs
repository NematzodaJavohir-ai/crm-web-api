using System;
using Domain.Enums;

namespace Application.Dtos.MentorDto;

public class MentorResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Specialization { get; set; }
    public int? ExperienceYears { get; set; }
    public string? Bio { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GithubUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsPasswordSet { get; set; }  
    public  InviteStatus  InviteStatus { get; set; }
    public DateTime HireDate { get; set; }
    public int ActiveGroupCount { get; set; }
}