using System.ComponentModel.DataAnnotations;

namespace BlazorApp.DTOs.GroupStudentDto;

public class AddStudentToGroupDto
{
    [Required]
    public int GroupId { get; set; }

    [Required]
    public int StudentId { get; set; }
}
