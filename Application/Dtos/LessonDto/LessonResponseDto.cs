using System;

namespace Application.Dtos.LessonDto;

public class LessonResponseDto
{
   public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public int WeekNumber { get; set; }
    public DateTime LessonDate { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? HomeworkDescription { get; set; }
    public string? MaterialUrl { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
