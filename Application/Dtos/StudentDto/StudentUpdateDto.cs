using System;

namespace Application.Dtos.StudentDto;

public class StudentUpdateDto
{
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? TelegramUsername { get; set; }
    public string? GithubUrl { get; set; }
    public string? AboutMe { get; set; }
}
