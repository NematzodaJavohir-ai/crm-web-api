using System;

namespace Application.Dtos.AttendanceDto;

public class AttendanceCreateDto
{
    public int LessonId { get; set; }
    public Guid StudentId { get; set; }
    public bool IsPresent { get; set; }
    public int Score { get; set; }
    public string? MentorNote { get; set; }
    public bool HomeworkDone { get; set; }
    public int HomeworkScore { get; set; }
}
