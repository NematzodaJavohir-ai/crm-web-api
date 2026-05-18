using Application.Dtos.LessonScoreDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface ILessonScoreService
{
    // ───── Basic CRUD ─────
    Task<Result<LessonScoreResponseDto>> SubmitAsync(LessonScoreCreateDto dto, CancellationToken ct = default);
    Task<Result<LessonScoreResponseDto>> UpdateAsync(int id, LessonScoreUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<LessonScoreResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);

    // ───── Mentor: Check ─────
    Task<Result<LessonScoreResponseDto>> CheckAsync(int id, int score, string? feedback, CancellationToken ct = default);
    Task<Result<IEnumerable<LessonScoreResponseDto>>> GetByHomeworkIdAsync(int homeworkId, CancellationToken ct = default);
    Task<Result<IEnumerable<LessonScoreResponseDto>>> GetUncheckedByHomeworkIdAsync(int homeworkId, CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<IEnumerable<LessonScoreResponseDto>>> GetMyScoresAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<double>> GetMyAverageScoreAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<int>> GetMyTotalScoreAsync(int userId, int groupId, CancellationToken ct = default);
}