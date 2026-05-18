using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.HomeworkDto;

public class HomeworkCreateDto
{
    [Required(ErrorMessage = "LessonId is required")]
    public int LessonId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string Description { get; set; } = null!;

    [Url(ErrorMessage = "FileUrl must be a valid URL")]
    [MaxLength(500, ErrorMessage = "FileUrl cannot exceed 500 characters")]
    public string? FileUrl { get; set; }

    [Required(ErrorMessage = "Deadline is required")]
    public DateTime Deadline { get; set; }

    [Range(1, 100, ErrorMessage = "MaxScore must be between 1 and 100")]
    public int MaxScore { get; set; } = 100;
}