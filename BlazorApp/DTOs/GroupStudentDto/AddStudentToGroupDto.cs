using System.ComponentModel.DataAnnotations;

namespace BlazorApp.DTOs.GroupStudentDto;

public class AddStudentToGroupDto
{
    [Required(ErrorMessage = "Student ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid Student ID")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Group ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid Group ID")]
    public int GroupId { get; set; }
}
