using Application.Dtos.LessonDto;
using Application.Results;
namespace Application.Interfaces.Services;

public interface ILessonService
{
    // ───── Admin + Mentor ─────
    Task<Result<LessonResponseDto>> CreateAsync(LessonCreateDto dto, CancellationToken ct = default);
    Task<Result<LessonResponseDto>> UpdateAsync(int id, LessonUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<LessonResponseDto>> MarkAsCompletedAsync(int id, CancellationToken ct = default);
    Task<Result<IEnumerable<LessonResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<Result<IEnumerable<LessonResponseDto>>> GetByWeekNumberAsync(int groupId, int weekNumber, CancellationToken ct = default);
    Task<Result<LessonWithAttendancesDto>> GetWithAttendancesAsync(int id, CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<IEnumerable<LessonResponseDto>>> GetMyLessonsAsync(int userId, int groupId, CancellationToken ct = default);
}