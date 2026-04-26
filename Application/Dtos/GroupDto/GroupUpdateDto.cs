using System;
using Domain.Enums;

namespace Application.Dtos.GroupDto;

public class GroupUpdateDto
{
      public string? Name { get; set; }
    public Guid? MentorId { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxStudents { get; set; }
    public string? Description { get; set; }
    public GroupStatus? Status { get; set; }
}
