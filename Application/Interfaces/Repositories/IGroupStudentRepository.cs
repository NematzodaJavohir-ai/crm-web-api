using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IGroupStudentRepository
{
   Task AddAsync(GroupStudent groupStudent, CancellationToken ct = default);
    void Update(GroupStudent groupStudent);
    void Delete(GroupStudent groupStudent);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    Task<GroupStudent?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<GroupStudent?> GetByGroupAndStudentAsync(int groupId, int studentId, CancellationToken ct = default);
    Task<IEnumerable<GroupStudent>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);             
    Task<IEnumerable<GroupStudent>> GetByStudentIdAsync(int studentId, CancellationToken ct = default);         
    Task<IEnumerable<GroupStudent>> GetActiveByGroupIdAsync(int groupId, CancellationToken ct = default);       
    Task<bool> IsStudentInGroupAsync(int groupId, int studentId, CancellationToken ct = default);
    Task RemoveStudentAsync(int groupId, int studentId, string? reason, CancellationToken ct = default);        // soft delete
    Task<int> GetActiveStudentCountAsync(int groupId, CancellationToken ct = default);
}
