using Application.Dtos.LessonDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface ILessonService
{
    // ───── Basic CRUD ─────
    Task<Result<LessonResponseDto>> CreateAsync(LessonCreateDto dto, CancellationToken ct = default);
    Task<Result<LessonResponseDto>> UpdateAsync(int id, LessonUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<LessonResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);

    // ───── Bulk Operations ─────
    Task<Result<IEnumerable<LessonResponseDto>>> BulkCreateAsync(BulkCreateLessonsDto dto, CancellationToken ct = default);
    Task<Result<bool>> BulkDeleteAsync(IEnumerable<int> ids, CancellationToken ct = default);

    // ───── Queries ─────
    Task<Result<IEnumerable<LessonResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<LessonResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<Result<IEnumerable<LessonResponseDto>>> GetByWeekNumberAsync(int groupId, int weekNumber, CancellationToken ct = default);
    Task<Result<IEnumerable<LessonResponseDto>>> GetByDateRangeAsync(int groupId, DateTime from, DateTime to, CancellationToken ct = default);

    // ───── Detailed ─────
    Task<Result<LessonWithAttendancesDto>> GetWithAttendancesAsync(int id, CancellationToken ct = default);
    Task<Result<LessonWithHomeworksDto>> GetWithHomeworksAsync(int id, CancellationToken ct = default);
    Task<Result<LessonFullDto>> GetFullAsync(int id, CancellationToken ct = default);

    // ───── Actions ─────
    Task<Result<LessonResponseDto>> MarkAsCompletedAsync(int id, CancellationToken ct = default);
    Task<Result<LessonResponseDto>> MarkAsNotCompletedAsync(int id, CancellationToken ct = default);
    Task<Result<LessonResponseDto>> CompleteLessonAsync(int id, CompleteLessonDto dto, CancellationToken ct = default);

    // ───── Queries: Stats ─────
    Task<Result<int>> GetCompletedCountAsync(int groupId, CancellationToken ct = default);
    Task<Result<int>> GetTotalCountAsync(int groupId, CancellationToken ct = default);
    Task<Result<LessonResponseDto>> GetNextLessonAsync(int groupId, CancellationToken ct = default);
    Task<Result<LessonResponseDto>> GetPreviousLessonAsync(int groupId, CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<IEnumerable<LessonResponseDto>>> GetMyLessonsAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<int>> GetMyCompletedCountAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<LessonResponseDto>> GetMyNextLessonAsync(int userId, int groupId, CancellationToken ct = default);
}