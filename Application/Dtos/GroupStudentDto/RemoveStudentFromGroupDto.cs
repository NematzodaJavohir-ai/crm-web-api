using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.GroupStudentDto;

public class RemoveStudentFromGroupDto
{
    [Required(ErrorMessage = "Student ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid Student ID")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Group ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid Group ID")]
    public int GroupId { get; set; }

    [MaxLength(500, ErrorMessage = "The reason cannot exceed 500 characters")]
    public string? Reason { get; set; }
}