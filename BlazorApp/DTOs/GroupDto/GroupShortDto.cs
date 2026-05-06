using System;
using BlazorApp.ApiServices.Enums;

namespace BlazorApp.DTOs.GroupDto;

public class GroupShortDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string MentorName { get; set; } = null!;
    public GroupStatus Status { get; set; }
}
