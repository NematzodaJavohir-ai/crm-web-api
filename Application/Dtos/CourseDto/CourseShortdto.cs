using System;

namespace Application.Dtos.CourseDto;

public class CourseShortDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int DurationWeeks { get; set; }
}
