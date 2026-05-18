namespace Domain.Entities;

public class LessonScore
{
    public int Id { get; set; }
    public int HomeworkId { get; set; }
    public int StudentId { get; set; }
    public int Score { get; set; }
    public string? SubmissionUrl { get; set; } 
    public string? MentorFeedback { get; set; } 
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Homework Homework { get; set; } = null!;
    public Student Student { get; set; } = null!;
}