using System.Text.RegularExpressions;

namespace Application.Interfaces.Repositories;

public interface IGroupRepository
{
     Task<IEnumerable<Group>> GetAllGroupsAsync(CancellationToken ct = default);
    Task<Group?> GetGroupByIdAsync(int id, CancellationToken ct = default);
    Task<int> AddGroupAsync(Group group, CancellationToken ct = default);
    Task<bool> UpdateGroup(Group group);
    Task<bool> DeleteGroup(Group group);
}
