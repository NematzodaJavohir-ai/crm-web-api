namespace Application.Dtos.GroupDto;

public class GroupResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public int MentorId { get; set; }
    public string MentorName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaxStudents { get; set; }
    public int ActiveStudents { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}