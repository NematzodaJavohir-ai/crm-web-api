using System;

namespace BlazorApp.DTOs.WeeklyResultDto;

public class WeekSummaryDto                         
{
    public int WeekNumber { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public IEnumerable<WeeklyResultResponseDto> Results { get; set; } = new List<WeeklyResultResponseDto>();
    public double GroupAverageScore { get; set; }
    public string TopStudentName { get; set; } = null!;
}
