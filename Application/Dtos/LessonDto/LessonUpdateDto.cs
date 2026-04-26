using System;

namespace Application.Dtos.LessonDto;

public class LessonUpdateDto
{
      public string? Title { get; set; }
    public string? Description { get; set; }
    public string? HomeworkDescription { get; set; }
    public string? MaterialUrl { get; set; }
    public bool? IsCompleted { get; set; }
}
