using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAttendanceRepository
{
    Task<IEnumerable<Attendance>> GetAttendanceByLessonIdAsync(int lessonId, CancellationToken ct = default);
    Task<int> AddAttendanceAsync(Attendance attendance, CancellationToken ct = default);
    Task<bool> UpdateAttendance(Attendance attendance);
}
