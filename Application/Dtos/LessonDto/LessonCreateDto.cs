using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.LessonDto;

public class LessonCreateDto
{
    [Required(ErrorMessage = "GroupId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid Group ID")]
    public int GroupId { get; set; }

    [Required(ErrorMessage = "Week number is required")]
    [Range(1, 52, ErrorMessage = "Week number must be between 1 and 52")]
    public int WeekNumber { get; set; }

    [Required(ErrorMessage = "Lesson date is required")]
    [DataType(DataType.DateTime)]
    public DateTime LessonDate { get; set; }

    [Required(ErrorMessage = "Lesson title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; set; } = null!;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [MaxLength(1000, ErrorMessage = "Homework description cannot exceed 1000 characters")]
    public string? HomeworkDescription { get; set; }

    [Url(ErrorMessage = "Material URL must be a valid URL")]
    public string? MaterialUrl { get; set; }
}