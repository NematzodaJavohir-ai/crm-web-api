using System;
using Application.Dtos.CourseDto;
using Application.Dtos.MentorDto;
using Domain.Enums;

namespace Application.Dtos.GroupDto;

public class GroupResponseDto
{
     public int Id { get; set; }
    public string Name { get; set; } = null!;
    public CourseResponseDto Course { get; set; } = null!;
    public MentorResponseDto Mentor { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaxStudents { get; set; }
    public int CurrentStudentCount { get; set; }
    public GroupStatus Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
