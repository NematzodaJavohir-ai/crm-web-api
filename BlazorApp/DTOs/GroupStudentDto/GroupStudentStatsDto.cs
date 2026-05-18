using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Dtos.GroupStudentDto;


public class TransferStudentDto
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int FromGroupId { get; set; }

    [Required]
    public int ToGroupId { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }
}
public class BulkAddStudentsDto
{
    [Required]
    public int GroupId { get; set; }
    [Required]
    [MinLength(1, ErrorMessage = "At least one student required")]
    public List<int> StudentIds { get; set; } = new();
}


public class GroupStudentHistoryDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public bool IsActive { get; set; }
    public string? RemoveReason { get; set; }
    public string Status => IsActive ? "Active" : "Left";
}



public class GroupStudentStatsDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int RemovedStudents { get; set; }
    public double AverageScore { get; set; }
    public DateTime? LastJoinedAt { get; set; }
}