using System;

namespace Application.Dtos.CourseDto;

public class CourseCreateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int DurationWeeks { get; set; }
}
