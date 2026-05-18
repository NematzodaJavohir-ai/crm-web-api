using Application.Dtos.AttendanceDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IAttendanceService
{
    // ───── Basic CRUD ─────
    Task<Result<AttendanceResponseDto>> CreateAsync(AttendanceCreateDto dto, CancellationToken ct = default);
    Task<Result<AttendanceResponseDto>> UpdateAsync(int id, AttendanceUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<AttendanceResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);

    // ───── Mentor: Lesson ─────
    Task<Result<IEnumerable<AttendanceResponseDto>>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default);
    Task<Result<bool>> MarkAllByLessonAsync(int lessonId, bool isPresent, CancellationToken ct = default);
    Task<Result<int>> GetPresentCountByLessonAsync(int lessonId, CancellationToken ct = default);
    Task<Result<int>> GetAbsentCountByLessonAsync(int lessonId, CancellationToken ct = default);

    // ───── Mentor: Week ─────
    Task<Result<IEnumerable<AttendanceResponseDto>>> GetByWeekAsync(int groupId, int weekNumber, CancellationToken ct = default);
    Task<Result<int>> GetPresentCountByWeekAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default);

    // ───── Mentor: Group ─────
    Task<Result<IEnumerable<AttendanceResponseDto>>> GetByGroupAsync(int groupId, CancellationToken ct = default);
    Task<Result<int>> GetAbsenceCountByGroupAsync(int studentId, int groupId, CancellationToken ct = default);
    Task<Result<double>> GetAttendanceRateByGroupAsync(int studentId, int groupId, CancellationToken ct = default);

    // ───── Mentor: Bulk ─────
    Task<Result<bool>> BulkCreateAsync(IEnumerable<AttendanceCreateDto> dtos, CancellationToken ct = default);
    Task<Result<bool>> BulkUpdateAsync(IEnumerable<AttendanceUpdateBulkDto> dtos, CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<bool>> AddAbsenceReasonAsync(int attendanceId, int userId, AddAbsenceReasonDto dto, CancellationToken ct = default);
    Task<Result<IEnumerable<AttendanceResponseDto>>> GetMyAttendancesAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<double>> GetMyAttendanceRateAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<int>> GetMyAbsenceCountAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<double>> GetMyAverageScoreAsync(int userId, int groupId, CancellationToken ct = default);
}