using Application.Dtos.LessonDto;

namespace Application.Dtos.GroupDto;

public class GroupWithStudentsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string MentorName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaxStudents { get; set; }
    public string Status { get; set; } = null!;
    public IEnumerable<GroupStudentDto> Students { get; set; } = new List<GroupStudentDto>();
}


public class GroupWithLessonsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string MentorName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = null!;
    public IEnumerable<LessonShortDto> Lessons { get; set; } = new List<LessonShortDto>();
}


public class GroupFullDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string MentorName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaxStudents { get; set; }
    public int ActiveStudents { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public IEnumerable<GroupStudentDto> Students { get; set; } = new List<GroupStudentDto>();
    public IEnumerable<LessonShortDto> Lessons { get; set; } = new List<LessonShortDto>();
}
public class GroupStudentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
}


