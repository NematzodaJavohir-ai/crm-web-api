using System;

namespace Application.Dtos.AttendanceDto;

public class AttendanceUpdateDto
{
    
    public bool? IsPresent { get; set; }
    public int? Score { get; set; }
    public string? MentorNote { get; set; }
    public bool? HomeworkDone { get; set; }
    public int? HomeworkScore { get; set; }
}
