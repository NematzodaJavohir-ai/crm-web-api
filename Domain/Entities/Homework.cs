namespace Domain.Entities;

public class Homework
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? FileUrl { get; set; } 
    public DateTime Deadline { get; set; }
    public int MaxScore { get; set; } = 100;

    public Lesson Lesson { get; set; } = null!;
    public ICollection<LessonScore> LessonScores { get; set; } = new List<LessonScore>();
 
}