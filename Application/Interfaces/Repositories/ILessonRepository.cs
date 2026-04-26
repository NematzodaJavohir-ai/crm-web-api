using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ILessonRepository
{
   Task AddAsync(Lesson lesson, CancellationToken ct = default);
    void Update(Lesson lesson);
    void Delete(Lesson lesson);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    Task<Lesson?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Lesson?> GetWithAttendancesAsync(int id, CancellationToken ct = default);  
    Task<IEnumerable<Lesson>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<IEnumerable<Lesson>> GetByWeekNumberAsync(int groupId, int weekNumber, CancellationToken ct = default);
    Task<Lesson?> GetByGroupAndDateAsync(int groupId, DateTime date, CancellationToken ct = default);
    Task<bool> DateExistsInGroupAsync(int groupId, DateTime date, CancellationToken ct = default);  
    Task<int> GetLastWeekNumberAsync(int groupId, CancellationToken ct = default);
}
