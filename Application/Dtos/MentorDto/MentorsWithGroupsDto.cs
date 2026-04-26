using System;
using Application.Dtos.GroupDto;

namespace Application.Dtos.MentorDto;

public class MentorWithGroupsDto : MentorResponseDto
{
    public IEnumerable<GroupShortDto> Groups { get; set; } = new List<GroupShortDto>();
}