using System.ComponentModel.DataAnnotations;

namespace BlazorApp.DTOs.WeeklyResultDto;

public class WeeklyResultCreateDto
{
    [Required(ErrorMessage = "Student ID is required")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Group ID is required")]
    public int GroupId { get; set; }

    [Required(ErrorMessage = "Week number is required")]
    [Range(1, 52, ErrorMessage = "Week number must be between 1 and 52")]
    public int WeekNumber { get; set; }

    [Range(0, 100, ErrorMessage = "Bonus score must be between 0 and 100")]
    public int BonusScore { get; set; }

    [Range(0, 100, ErrorMessage = "Exam score must be between 0 and 100")]
    public int ExamScore { get; set; }

    [MaxLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
    public string? MentorComment { get; set; }
}
