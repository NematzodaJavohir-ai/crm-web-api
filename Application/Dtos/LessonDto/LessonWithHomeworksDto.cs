using Application.Dtos;

namespace Application.Dtos.LessonDto;

public class LessonWithHomeworksDto
{
    public int Id { get; set; }
    public int WeekNumber { get; set; }
    public DateTime LessonDate { get; set; }
    public string? Title { get; set; }
    public bool IsCompleted { get; set; }
    public IEnumerable<HomeworkResponseDto> Homeworks { get; set; } = new List<HomeworkResponseDto>();
}


public class HomeworkShortDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime Deadline { get; set; }
    public int MaxScore { get; set; }
    public int SubmittedCount { get; set; }
    public int CheckedCount { get; set; }
}
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