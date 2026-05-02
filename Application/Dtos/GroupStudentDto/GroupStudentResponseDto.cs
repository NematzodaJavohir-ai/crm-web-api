using System;

namespace Application.Dtos.GroupStudentDto;

public class GroupStudentResponseDto
{
   public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public string StudentEmail { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public bool IsActive { get; set; }
    public string? RemoveReason { get; set; }
}
