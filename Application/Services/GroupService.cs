using System;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class GroupService(IUnitOfWork uow)
{
  public async Task CreateGroupWithStudentAsync(Group group, GroupStudent student)
    {
        await uow.Groups.AddAsync(group);
        await uow.GroupStudents.AddAsync(student);
        await uow.SaveChangesAsync(); 
    }
}
