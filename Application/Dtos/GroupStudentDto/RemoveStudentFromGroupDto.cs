using System;

namespace Application.Dtos.GroupStudentDto;

public class RemoveStudentFromGroupDto
{
   public Guid StudentId { get; set; }
    public int GroupId { get; set; }
    public string? Reason { get; set; }
}
