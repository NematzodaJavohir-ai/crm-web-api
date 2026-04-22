namespace Domain.Entities;

public class Attendance
{
    public int Id { get; set; } 
    public int LessonId { get; set; }
    public int StudentId { get; set; }
    public bool IsPresent { get; set; } = false;
    public int Score { get; set; } = 0;              
    public string? AbsenceReason { get; set; }        
    public string? MentorNote { get; set; }
    public bool HomeworkDone { get; set; } = false;
    public int HomeworkScore { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Lesson Lesson { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
