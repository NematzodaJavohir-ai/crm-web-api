using System;

namespace Application.Dtos.GroupDto;

public class GroupCreateDto
{
     public string Name { get; set; } = null!;
    public Guid CourseId { get; set; }
    public Guid MentorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaxStudents { get; set; } = 15;
    public string? Description { get; set; }
}
