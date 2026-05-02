using System;
using Application.Dtos.GroupDto;

namespace Application.Dtos.CourseDto;

public class CourseResponseDto
{
      public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int DurationWeeks { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CourseWithGroupsDto : CourseResponseDto
{
    public IEnumerable<GroupShortDto> Groups { get; set; } = new List<GroupShortDto>();
}
