using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.LessonDto;

public class LessonUpdateDto
{
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string? Title { get; set; }

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [MaxLength(1000, ErrorMessage = "Homework description cannot exceed 1000 characters")]
    public string? HomeworkDescription { get; set; }

    [Url(ErrorMessage = "Material URL must be a valid URL")]
    public string? MaterialUrl { get; set; }

    public bool? IsCompleted { get; set; }
}