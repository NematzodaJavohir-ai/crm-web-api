using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IWeekResultRepository
{
    Task AddAsync(WeekResult weekResult, CancellationToken ct = default);
    void Update(WeekResult weekResult);
    void Delete(WeekResult weekResult);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    Task<WeekResult?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<WeekResult?> GetByStudentGroupWeekAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default);
    Task<IEnumerable<WeekResult>> GetByStudentIdAsync(int studentId, CancellationToken ct = default);
    Task<IEnumerable<WeekResult>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<IEnumerable<WeekResult>> GetByGroupAndWeekAsync(int groupId, int weekNumber, CancellationToken ct = default); 
    Task<WeekResult?> GetBestWeekAsync(int studentId, int groupId, CancellationToken ct = default); 
    Task<double> GetAverageScoreAsync(int studentId, int groupId, CancellationToken ct = default); 
    Task RecalculateTotalAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default); 
}
