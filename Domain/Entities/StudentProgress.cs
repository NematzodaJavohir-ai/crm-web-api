namespace Domain.Entities;

public class StudentProgress
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int GroupId { get; set; }
    public double AttendanceRate { get; set; } 
    public double AverageHomeworkScore { get; set; } 
    public bool IsRecommendedForCertificate { get; set; }

    public Student Student { get; set; } = null!;
    public Group Group { get; set; } = null!;
}