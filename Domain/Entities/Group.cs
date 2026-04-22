using Domain.Enums;

namespace Domain.Entities;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;   
    public int CourseId { get; set; }
    public int MentorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaxStudents { get; set; } = 15;
    public GroupStatus Status { get; set; } = GroupStatus.Active;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Course Course { get; set; } = null!;
    public Mentor Mentor { get; set; } = null!;
    public ICollection<GroupStudent> GroupStudents { get; set; } = new List<GroupStudent>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<WeekResult> WeekResults { get; set; } = new List<WeekResult>();
}
