using System;
using System.ComponentModel.DataAnnotations;
namespace Application.Dtos.GroupStudentDto;

public class TransferStudentDto
{
    [Required(ErrorMessage = "StudentId is required")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "FromGroupId is required")]
    public int FromGroupId { get; set; }

    [Required(ErrorMessage = "ToGroupId is required")]
    public int ToGroupId { get; set; }

    [MaxLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string? Reason { get; set; }
}




public class BulkAddStudentsDto
{
    [Required(ErrorMessage = "GroupId is required")]
    public int GroupId { get; set; }

    [Required(ErrorMessage = "StudentIds is required")]
    [MinLength(1, ErrorMessage = "At least one student is required")]
    public IEnumerable<int> StudentIds { get; set; } = new List<int>();
}

// History


public class GroupStudentHistoryDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string MentorName { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public bool IsActive { get; set; }
    public string? RemoveReason { get; set; }
    public bool IsTransferred { get; set; }
    public string? TransferredToGroup { get; set; }
    public string? TransferredFromGroup { get; set; }
}

// Stat of Grouuuuup



public class GroupStudentStatsDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public int MaxStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int TotalJoined { get; set; }
    public int TotalLeft { get; set; }
    public int TransferredIn { get; set; }
    public int TransferredOut { get; set; }
    public double RetentionRate { get; set; }
    public int AvailableSeats { get; set; }
}

public class BulkRemoveStudentsDto
{
    [Required(ErrorMessage = "GroupId is required")]
    public int GroupId { get; set; }

    [Required(ErrorMessage = "StudentIds is required")]
    [MinLength(1, ErrorMessage = "At least one student is required")]
    public IEnumerable<int> StudentIds { get; set; } = new List<int>();

    [MaxLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string? Reason { get; set; }
}
public class TransferResultDto
{
    public bool Success { get; set; }
    public GroupStudentResponseDto? FromGroup { get; set; }
    public GroupStudentResponseDto? ToGroup { get; set; }
    public string? Message { get; set; }
}

public class BulkOperationResultDto
{
    public int TotalRequested { get; set; }
    public int Succeeded { get; set; }
    public int Failed { get; set; }
    public IEnumerable<BulkOperationErrorDto> Errors { get; set; } = new List<BulkOperationErrorDto>();
}
public class BulkOperationErrorDto
{
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? Reason { get; set; }
}
public class TransferHistoryDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public int FromGroupId { get; set; }
    public string? FromGroupName { get; set; }
    public int ToGroupId { get; set; }
    public string? ToGroupName { get; set; }
    public DateTime TransferDate { get; set; }
    public string? Reason { get; set; }
}

