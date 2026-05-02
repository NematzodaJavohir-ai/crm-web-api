using System;

namespace Application.Dtos.AttendanceDto;

public class AttendanceResponseDto
{
     public int Id { get; set; }
    public int LessonId { get; set; }
    public DateTime LessonDate { get; set; }
    public int WeekNumber { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public bool IsPresent { get; set; }
    public int Score { get; set; }
    public string? AbsenceReason { get; set; }
    public string? MentorNote { get; set; }
    public bool HomeworkDone { get; set; }
    public int HomeworkScore { get; set; }
    public DateTime CreatedAt { get; set; }
}
