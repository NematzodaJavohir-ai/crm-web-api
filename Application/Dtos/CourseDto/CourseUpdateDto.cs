using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.CourseDto;

public class CourseUpdateDto
{
    [Required(ErrorMessage = "Course name is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
    public string Name { get; set; } = null!;

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0, 9999999.99, ErrorMessage = "Price must be a positive value")]
    public decimal Price { get; set; }

    [Url(ErrorMessage = "IconUrl must be a valid URL")]
    [MaxLength(500, ErrorMessage = "IconUrl cannot exceed 500 characters")]
    public string? IconUrl { get; set; }

    [Required(ErrorMessage = "Duration is required")]
    [Range(1, 52, ErrorMessage = "Duration must be between 1 and 52 weeks")]
    public int DurationWeeks { get; set; }

    public bool IsActive { get; set; }
}