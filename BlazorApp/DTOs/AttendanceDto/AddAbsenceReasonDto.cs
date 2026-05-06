using System.ComponentModel.DataAnnotations;

namespace BlazorApp.DTOs.AttendanceDto;

public class AddAbsenceReasonDto 
{
    [Required(ErrorMessage = "Reason is required")]
    [MinLength(3, ErrorMessage = "Reason must be at least 3 characters long")]
    [MaxLength(200, ErrorMessage = "Reason cannot exceed 200 characters")]
    public string AbsenceReason { get; set; } = null!;
}
