using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IGroupRepository
{
    Task AddAsync(Group group, CancellationToken ct = default);
    Task<IEnumerable<Group>> GetAllAsync(CancellationToken ct = default);
    void Update(Group group);
    void Delete(Group group);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
 

    Task<Group?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Group?> GetWithStudentsAsync(int id, CancellationToken ct = default);
    Task<Group?> GetWithLessonsAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Group>> GetByCourseIdAsync(int courseId, CancellationToken ct = default);
    Task<IEnumerable<Group>> GetByMentorIdAsync(int mentorId, CancellationToken ct = default);
    Task<IEnumerable<Group>> GetActiveAsync(CancellationToken ct = default);
    Task<IEnumerable<Group>> GetByStatusAsync(GroupStatus status, CancellationToken ct = default);
    Task<int> GetStudentCountAsync(int id, CancellationToken ct = default);
    Task<bool> HasAvailableSeatsAsync(int id, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
}