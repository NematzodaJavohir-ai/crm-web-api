namespace Application.Dtos.HomeworkDto;

public class HomeworkResponseDto
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public int WeekNumber { get; set; }
    public DateTime LessonDate { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? FileUrl { get; set; }
    public DateTime Deadline { get; set; }
    public int MaxScore { get; set; }
    public int SubmittedCount { get; set; }
    public int CheckedCount { get; set; }
}