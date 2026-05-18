using System;

namespace Application.Dtos.MentorDto;

public class MentorShortDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Specialization { get; set; }
    public string? PhotoUrl { get; set; }
    public int ActiveGroupCount { get; set; }
}
