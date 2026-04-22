namespace Domain.Entities;

public class WeekResult
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int GroupId { get; set; }
    public int WeekNumber { get; set; }
    public int BonusScore { get; set; } = 0;
    public int ExamScore { get; set; } = 0;
    public int AttendanceScore { get; set; } = 0;     
    public int TotalScore { get; set; } = 0;          
    public string? MentorComment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set;}

    // Navigation
    public Student Student { get; set; } = null!;
    public Group Group { get; set; } = null!;
}
