using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.LessonScoreDto;

public class LessonScoreCreateDto
{
    [Required(ErrorMessage = "HomeworkId is required")]
    public int HomeworkId { get; set; }

    [Required(ErrorMessage = "StudentId is required")]
    public int StudentId { get; set; }

    [Url(ErrorMessage = "SubmissionUrl must be a valid URL")]
    [MaxLength(500, ErrorMessage = "SubmissionUrl cannot exceed 500 characters")]
    public string? SubmissionUrl { get; set; }
}