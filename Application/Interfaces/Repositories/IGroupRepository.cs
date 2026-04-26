

using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IGroupRepository
{
    Task AddAsync(Group group, CancellationToken ct = default);
    void Update(Group group);
    void Delete(Group group);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    Task<Group?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Group?> GetWithDetailsAsync(int id, CancellationToken ct = default); 
    Task<IEnumerable<Group>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Group>> GetAllActiveAsync(CancellationToken ct = default);
    Task<IEnumerable<Group>> GetByMentorIdAsync(int mentorId, CancellationToken ct = default);
    Task<IEnumerable<Group>> GetByCourseIdAsync(int courseId, CancellationToken ct = default);
    Task<int> GetStudentCountAsync(int groupId, CancellationToken ct = default);
}
