namespace Application.Dtos.LessonScoreDto;

public class LessonScoreResponseDto
{
    public int Id { get; set; }
    public int HomeworkId { get; set; }
    public string HomeworkTitle { get; set; } = null!;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public int Score { get; set; }
    public string? SubmissionUrl { get; set; }
    public string? MentorFeedback { get; set; }
    public DateTime SubmittedAt { get; set; }
}