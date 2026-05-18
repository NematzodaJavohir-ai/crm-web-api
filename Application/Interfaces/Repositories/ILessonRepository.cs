using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ILessonRepository
{
    Task AddAsync(Lesson lesson, CancellationToken ct = default);
    void Update(Lesson lesson);
    void Delete(Lesson lesson);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
     Task<IEnumerable<Lesson>> GetAllAsync(CancellationToken ct = default);
  

    Task<Lesson?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Lesson?> GetWithAttendanceAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Lesson>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<IEnumerable<Lesson>> GetByWeekAsync(int groupId, int weekNumber, CancellationToken ct = default);
    Task<IEnumerable<Lesson>> GetByDateRangeAsync(int groupId, DateTime from, DateTime to, CancellationToken ct = default);
    Task<int> GetCompletedCountAsync(int groupId, CancellationToken ct = default);
    Task<Lesson?> GetNextLessonAsync(int groupId, CancellationToken ct = default);
}