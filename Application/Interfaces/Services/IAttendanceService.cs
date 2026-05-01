using Application.Dtos.AttendanceDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IAttendanceService
{
    // ───── Mentor ─────
    Task<Result<AttendanceResponseDto>> CreateAsync(AttendanceCreateDto dto, CancellationToken ct = default);
    Task<Result<AttendanceResponseDto>> UpdateAsync(int id, AttendanceUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<IEnumerable<AttendanceResponseDto>>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default);
    Task<Result<IEnumerable<AttendanceResponseDto>>> GetByWeekAsync(int groupId, int weekNumber, CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<bool>> AddAbsenceReasonAsync(int attendanceId, int userId, AddAbsenceReasonDto dto, CancellationToken ct = default);
    Task<Result<IEnumerable<AttendanceResponseDto>>> GetMyAttendancesAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<double>> GetMyAverageScoreAsync(int userId, int groupId, CancellationToken ct = default);
    Task<Result<int>> GetMyAbsenceCountAsync(int userId, int groupId, CancellationToken ct = default);
}