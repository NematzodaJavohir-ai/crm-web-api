using System;

namespace Application.Dtos.WeeklyResultDto;

public class WeeklyResultUpdateDto
{
      public int? BonusScore { get; set; }
    public int? ExamScore { get; set; }
    public string? MentorComment { get; set; }
}
