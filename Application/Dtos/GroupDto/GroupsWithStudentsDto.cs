using System;
using Application.Dtos.StudentDto;

namespace Application.Dtos.GroupDto;

public class GroupWithStudentsDto : GroupResponseDto
{
    public IEnumerable<StudentResponseDto> Students { get; set; } = new List<StudentResponseDto>();
}