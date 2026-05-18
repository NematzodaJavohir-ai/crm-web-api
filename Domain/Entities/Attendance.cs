namespace Domain.Entities;

public class Attendance
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public int StudentId { get; set; }
    public bool IsPresent { get; set; }
    public string? AbsenceReason { get; set; }
    public string? MentorNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Lesson Lesson { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
