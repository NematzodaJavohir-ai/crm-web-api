using Application.Dtos.HomeworkDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IHomeworkService
{
    // ───── Basic CRUD ─────
    Task<Result<HomeworkResponseDto>> CreateAsync(HomeworkCreateDto dto, CancellationToken ct = default);
    Task<Result<HomeworkResponseDto>> UpdateAsync(int id, HomeworkUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<HomeworkResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);

    // ───── Queries ─────
    Task<Result<IEnumerable<HomeworkResponseDto>>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default);
    Task<Result<IEnumerable<HomeworkResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<Result<IEnumerable<HomeworkResponseDto>>> GetOverdueAsync(CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<IEnumerable<HomeworkResponseDto>>> GetMyHomeworksAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<IEnumerable<HomeworkResponseDto>>> GetMyOverdueAsync(int userId, int groupId, CancellationToken ct = default);
}