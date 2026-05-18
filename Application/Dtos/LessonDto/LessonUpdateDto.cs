using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.LessonDto;

public class LessonUpdateDto
{
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; set; }

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [MaxLength(2000, ErrorMessage = "HomeworkDescription cannot exceed 2000 characters")]
    public string? HomeworkDescription { get; set; }

    [Url(ErrorMessage = "MaterialUrl must be a valid URL")]
    [MaxLength(500, ErrorMessage = "MaterialUrl cannot exceed 500 characters")]
    public string? MaterialUrl { get; set; }
}