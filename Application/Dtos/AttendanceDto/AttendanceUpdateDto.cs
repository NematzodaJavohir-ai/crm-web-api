using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.AttendanceDto;

public class AttendanceUpdateDto
{
    public bool? IsPresent { get; set; }

    [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
    public int? Score { get; set; }

    [MaxLength(500, ErrorMessage = "Mentor note cannot exceed 500 characters")]
    public string? MentorNote { get; set; }

    public bool? HomeworkDone { get; set; }

    [Range(0, 100, ErrorMessage = "Homework score must be between 0 and 100")]
    public int? HomeworkScore { get; set; }
}