using System;

namespace Application.Dtos.LessonDto;

public class LessonCreateDto
{
    public int GroupId { get; set; }
    public int WeekNumber { get; set; }
    public DateTime LessonDate { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? HomeworkDescription { get; set; }
    public string? MaterialUrl { get; set; }
}
