using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAttendanceRepository
{
    Task AddAsync(Attendance attendance, CancellationToken ct = default);
    void Update(Attendance attendance);
    void Delete(Attendance attendance);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    Task<Attendance?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Attendance?> GetByLessonAndStudentAsync(int lessonId, int studentId, CancellationToken ct = default);  
    Task<IEnumerable<Attendance>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default);              
    Task<IEnumerable<Attendance>> GetByStudentIdAsync(int studentId, CancellationToken ct = default);          
    Task<IEnumerable<Attendance>> GetByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default);
    Task<IEnumerable<Attendance>> GetByWeekAsync(int groupId, int weekNumber, CancellationToken ct = default);  
    Task<bool> AlreadyExistsAsync(int lessonId, int studentId, CancellationToken ct = default);                
    Task<int> GetTotalScoreByWeekAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default);
    Task<double> GetAverageScoreAsync(int studentId, int groupId, CancellationToken ct = default);
    Task<int> GetAbsenceCountAsync(int studentId, int groupId, CancellationToken ct = default);                
}
