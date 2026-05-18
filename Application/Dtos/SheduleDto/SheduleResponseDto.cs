namespace Application.Dtos.SheduleDto;

public class SheduleResponseDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string Day { get; set; } = null!;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public bool IsOnline { get; set; }
}