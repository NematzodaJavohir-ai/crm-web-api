using System.ComponentModel.DataAnnotations;

namespace BlazorApp.DTOs.WeeklyResultDto;

public class WeeklyResultUpdateDto
{
    [Range(0, 100, ErrorMessage = "Bonus score must be between 0 and 100")]
    public int? BonusScore { get; set; }

    [Range(0, 100, ErrorMessage = "Exam score must be between 0 and 100")]
    public int? ExamScore { get; set; }

    [MaxLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
    public string? MentorComment { get; set; }
}
