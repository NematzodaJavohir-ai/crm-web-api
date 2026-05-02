using System;
namespace Application.Dtos.WeeklyResultDto;

public class WeeklyResultResponseDto
{
      public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public int WeekNumber { get; set; }
    public int AttendanceScore { get; set; }
    public int BonusScore { get; set; }
    public int ExamScore { get; set; }
    public int TotalScore { get; set; }
    public string? MentorComment { get; set; }
    public DateTime CreatedAt { get; set; }
}
