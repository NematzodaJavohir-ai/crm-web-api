using System;
using BlazorApp.DTOs.StudentDto;


namespace BlazorApp.DTOs.GroupDto;

public class GroupWithStudentsDto : GroupResponseDto
{
    public IEnumerable<StudentResponseDto> Students { get; set; } = new List<StudentResponseDto>();
}
