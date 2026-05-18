using Application.Dtos.WeeklyResultDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IWeekResultService
{
    // ───── Basic CRUD ─────
    Task<Result<WeeklyResultResponseDto>> CreateAsync(WeeklyResultCreateDto dto, CancellationToken ct = default);
    Task<Result<WeeklyResultResponseDto>> UpdateAsync(int id, WeeklyResultUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<WeeklyResultResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);

    // ───── Mentor: Group & Week ─────
    Task<Result<IEnumerable<WeeklyResultResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<Result<WeekSummaryDto>> GetByGroupAndWeekAsync(int groupId, int weekNumber, CancellationToken ct = default);
    Task<Result<IEnumerable<WeeklyResultResponseDto>>> GetByStudentIdAsync(int studentId, CancellationToken ct = default);

    // ───── Mentor: Actions ─────
    Task<Result<bool>> RecalculateTotalAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default);
    Task<Result<bool>> BulkCreateAsync(int groupId, int weekNumber, CancellationToken ct = default);
    Task<Result<bool>> BulkUpdateAsync(int groupId, int weekNumber, WeeklyResultUpdateDto dto, CancellationToken ct = default);

    // ───── Mentor: Stats ─────
    Task<Result<int>> GetTotalScoreByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default);
    Task<Result<double>> GetAverageTotalScoreByGroupAsync(int groupId, CancellationToken ct = default);
    Task<Result<IEnumerable<WeeklyResultResponseDto>>> GetTopByWeekAsync(int groupId, int weekNumber, int topCount, CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<IEnumerable<WeeklyResultResponseDto>>> GetMyResultsAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<WeeklyResultResponseDto>> GetMyResultByWeekAsync(int userId, int groupId, int weekNumber, CancellationToken ct = default);
    Task<Result<double>> GetMyAverageScoreAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<int>> GetMyTotalScoreAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<WeeklyResultResponseDto>> GetMyBestWeekAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<WeeklyResultResponseDto>> GetMyWorstWeekAsync(int userId, int groupId, CancellationToken ct = default);
}