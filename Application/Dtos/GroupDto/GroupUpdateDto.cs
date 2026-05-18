using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.GroupDto;

public class GroupUpdateDto
{
    [Required(ErrorMessage = "Group name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "MentorId is required")]
    public int MentorId { get; set; }

    public DateTime? EndDate { get; set; }

    [Range(5, 50, ErrorMessage = "MaxStudents must be between 5 and 50")]
    public int MaxStudents { get; set; }
}