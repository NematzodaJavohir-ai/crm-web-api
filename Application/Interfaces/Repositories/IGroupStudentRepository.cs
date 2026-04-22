using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IGroupStudentRepository
{
    Task<IEnumerable<GroupStudent>> GetStudentsByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<int> AddStudentToGroupAsync(GroupStudent groupStudent, CancellationToken ct = default);
    Task<bool> RemoveStudentFromGroup(GroupStudent groupStudent);
}
