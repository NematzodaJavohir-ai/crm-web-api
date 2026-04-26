using System;

namespace Application.Dtos.WeeklyResultDto;

public class WeeklyResultCreateDto
{
    public Guid StudentId { get; set; }
    public int GroupId { get; set; }
    public int WeekNumber { get; set; }
    public int BonusScore { get; set; }
    public int ExamScore { get; set; }
    public string? MentorComment { get; set; }
}
