using Application.Dtos.SheduleDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface ISheduleService
{
    // ───── Basic CRUD ─────
    Task<Result<SheduleResponseDto>> CreateAsync(SheduleCreateDto dto, CancellationToken ct = default);
    Task<Result<SheduleResponseDto>> UpdateAsync(int id, SheduleUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<SheduleResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);

    // ───── Queries ─────
    Task<Result<IEnumerable<SheduleResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<Result<IEnumerable<SheduleResponseDto>>> GetByDayAsync(DayOfWeek day, CancellationToken ct = default);

    // ───── Validation ─────
    Task<Result<bool>> HasConflictAsync(int groupId, DayOfWeek day, TimeSpan start, TimeSpan end, CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<IEnumerable<SheduleResponseDto>>> GetMyScheduleAsync(int userId, CancellationToken ct = default);
}