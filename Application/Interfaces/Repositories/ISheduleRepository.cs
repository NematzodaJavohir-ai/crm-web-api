using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISheduleRepository
{
    Task AddAsync(Shedule shedule, CancellationToken ct = default);
    void Update(Shedule shedule);
    void Delete(Shedule shedule);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
  

    Task<Shedule?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Shedule>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<IEnumerable<Shedule>> GetByDayAsync(DayOfWeek day, CancellationToken ct = default);
    Task<bool> HasConflictAsync(int groupId, DayOfWeek day, TimeSpan start, TimeSpan end, CancellationToken ct = default);
}