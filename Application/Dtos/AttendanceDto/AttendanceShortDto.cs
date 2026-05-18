namespace Application.Dtos.AttendanceDto;

public class AttendanceShortDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public bool IsPresent { get; set; }
    public string? MentorNote { get; set; }
}