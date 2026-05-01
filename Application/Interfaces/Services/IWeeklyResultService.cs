using Application.Dtos.WeeklyResultDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IWeekResultService
{
    // ───── Mentor ─────
    Task<Result<WeeklyResultResponseDto>> CreateAsync(WeeklyResultCreateDto dto, CancellationToken ct = default);
    Task<Result<WeeklyResultResponseDto>> UpdateAsync(int id, WeeklyResultUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<WeekSummaryDto>> GetByGroupAndWeekAsync(int groupId, int weekNumber, CancellationToken ct = default);
    Task<Result<IEnumerable<WeeklyResultResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<Result<bool>> RecalculateTotalAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<IEnumerable<WeeklyResultResponseDto>>> GetMyResultsAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<WeeklyResultResponseDto>> GetMyBestWeekAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<double>> GetMyAverageScoreAsync(int userId, int groupId, CancellationToken ct = default);
}