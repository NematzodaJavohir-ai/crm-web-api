using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.MentorDto;

public class MentorCreateDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Phone]
    public string? Phone { get; set; }

    public string? Specialization { get; set; }
}