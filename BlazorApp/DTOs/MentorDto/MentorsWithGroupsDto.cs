using System;
using BlazorApp.DTOs.GroupDto;

namespace BlazorApp.DTOs.MentorDto;

public class MentorWithGroupsDto : MentorResponseDto
{
    public IEnumerable<GroupShortDto> Groups { get; set; } = new List<GroupShortDto>();
}
