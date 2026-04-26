using System;

namespace Application.Dtos.GroupStudentDto;

public class AddStudentToGroup
{
     
    public Guid StudentId { get; set; }
    public int GroupId { get; set; }
}   
