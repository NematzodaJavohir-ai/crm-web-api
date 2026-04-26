using System;

namespace Application.Dtos.CourseDto;

public class CourseUpdateDto
{
   
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int? DurationWeeks { get; set; }
    public bool? IsActive { get; set; }
}
