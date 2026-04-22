using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IWeekResultRepository
{
    Task<WeekResult?> GetResultAsync(int studentId, int weekNumber, CancellationToken ct = default);
    Task<int> AddWeekResultAsync(WeekResult result, CancellationToken ct = default);
    Task<bool> UpdateWeekResult(WeekResult result);
}
