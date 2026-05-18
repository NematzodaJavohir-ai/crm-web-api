using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.LessonScoreDto;

public class LessonScoreUpdateDto
{
    [Url(ErrorMessage = "SubmissionUrl must be a valid URL")]
    [MaxLength(500, ErrorMessage = "SubmissionUrl cannot exceed 500 characters")]
    public string? SubmissionUrl { get; set; }
}