using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.AttendanceDto;

public class AttendanceCreateDto
{
    [Required(ErrorMessage = "LessonId is required")]
    public int LessonId { get; set; }

    [Required(ErrorMessage = "StudentId is required")]
    public int StudentId { get; set; }

    public bool IsPresent { get; set; }

    [MaxLength(500, ErrorMessage = "Absence reason cannot exceed 500 characters")]
    public string? AbsenceReason { get; set; }

    [MaxLength(500, ErrorMessage = "Mentor note cannot exceed 500 characters")]
    public string? MentorNote { get; set; }
}