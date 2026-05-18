using Application.Dtos.AttendanceDto;

namespace Application.Dtos.LessonDto;

public class LessonFullDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string MentorName { get; set; } = null!;
    public int WeekNumber { get; set; }
    public DateTime LessonDate { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? HomeworkDescription { get; set; }
    public string? MaterialUrl { get; set; }
    public bool IsCompleted { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<AttendanceShortDto> Attendances { get; set; } = new List<AttendanceShortDto>();
    public IEnumerable<HomeworkShortDto> Homeworks { get; set; } = new List<HomeworkShortDto>();
}