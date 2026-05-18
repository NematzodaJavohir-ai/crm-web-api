using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IHomeworkRepository
{
    Task AddAsync(Homework homework, CancellationToken ct = default);
    void Update(Homework homework);
    void Delete(Homework homework);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);


    Task<Homework?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Homework>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default);
    Task<IEnumerable<Homework>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<IEnumerable<Homework>> GetByDeadlineAsync(DateTime deadline, CancellationToken ct = default);
    Task<IEnumerable<Homework>> GetOverdueAsync(CancellationToken ct = default);
}