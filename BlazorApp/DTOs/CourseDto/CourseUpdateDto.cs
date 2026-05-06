using System.ComponentModel.DataAnnotations;

namespace BlazorApp.DTOs.CourseDto;

public class CourseUpdateDto
{
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
    public string? Name { get; set; }

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Url(ErrorMessage = "IconUrl must be a valid URL")]
    public string? IconUrl { get; set; }

    [Range(1, 52, ErrorMessage = "Duration must be between 1 and 52 weeks")]
    public int? DurationWeeks { get; set; }

    public bool? IsActive { get; set; }
}
