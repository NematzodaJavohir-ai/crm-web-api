using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.AttendanceDto;

public class AttendanceUpdateDto
{
    public bool IsPresent { get; set; }

    [MaxLength(500, ErrorMessage = "Absence reason cannot exceed 500 characters")]
    public string? AbsenceReason { get; set; }

    [MaxLength(500, ErrorMessage = "Mentor note cannot exceed 500 characters")]
    public string? MentorNote { get; set; }
}
public class AttendanceUpdateBulkDto
{
    [Required(ErrorMessage = "AttendanceId is required")]
    public int AttendanceId { get; set; }

    public bool IsPresent { get; set; }

    [MaxLength(500, ErrorMessage = "Mentor note cannot exceed 500 characters")]
    public string? MentorNote { get; set; }
}