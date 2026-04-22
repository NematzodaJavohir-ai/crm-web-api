namespace Domain.Entities;
public class Student
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? AboutMe { get; set; }
    public string? GithubUrl { get; set; }
    public string? TelegramUsername { get; set; }
    public DateTime EnrollDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<GroupStudent> GroupStudents { get; set; } = new List<GroupStudent>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<WeekResult> WeekResults { get; set; } = new List<WeekResult>();
}