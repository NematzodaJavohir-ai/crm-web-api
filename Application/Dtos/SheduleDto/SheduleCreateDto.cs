using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.SheduleDto;

public class SheduleCreateDto
{
    [Required(ErrorMessage = "GroupId is required")]
    public int GroupId { get; set; }

    [Required(ErrorMessage = "Day is required")]
    public DayOfWeek Day { get; set; }

    [Required(ErrorMessage = "StartTime is required")]
    public TimeSpan StartTime { get; set; }

    [Required(ErrorMessage = "EndTime is required")]
    public TimeSpan EndTime { get; set; }

    [MaxLength(100, ErrorMessage = "Room cannot exceed 100 characters")]
    public string? Room { get; set; }

    public bool IsOnline { get; set; }
}