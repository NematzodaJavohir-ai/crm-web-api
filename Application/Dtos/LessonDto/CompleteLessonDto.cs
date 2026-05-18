using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.LessonDto;

public class CompleteLessonDto
{
    [Required(ErrorMessage = "IsCompleted is required")]
    public bool IsCompleted { get; set; }

    [MaxLength(500, ErrorMessage = "Summary cannot exceed 500 characters")]
    public string? Summary { get; set; }
}


public class BulkCreateLessonsDto
{
    [Required(ErrorMessage = "GroupId is required")]
    public int GroupId { get; set; }

    [Required(ErrorMessage = "Lessons is required")]
    [MinLength(1, ErrorMessage = "At least one lesson is required")]
    public IEnumerable<BulkLessonItemDto> Lessons { get; set; } = new List<BulkLessonItemDto>();
}

public class BulkLessonItemDto
{
    [Required]
    [Range(1, 52)]
    public int WeekNumber { get; set; }

    [Required]
    public DateTime LessonDate { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }
}