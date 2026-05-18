using System;

namespace Application.Dtos.StudentDto;



public class StudentShortDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhotoUrl { get; set; }
    public int ActiveGroupsCount { get; set; }
}
