using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.GroupDto;

public class GroupCreateDto
{
    [Required(ErrorMessage = "Group name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Course selection is required")]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Mentor assignment is required")]
    public int MentorId { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Required]
    [Range(1, 50, ErrorMessage = "Max students must be between 1 and 50")]
    public int MaxStudents { get; set; } = 15;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}