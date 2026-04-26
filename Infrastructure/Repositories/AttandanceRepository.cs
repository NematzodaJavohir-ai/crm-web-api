using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AttendanceRepository(DataContext context) : IAttendanceRepository
{
    public async Task AddAsync(Attendance attendance, CancellationToken ct = default)
        => await context.Attendances.AddAsync(attendance, ct);

    public void Update(Attendance attendance)
        => context.Attendances.Update(attendance);

    public void Delete(Attendance attendance)
        => context.Attendances.Remove(attendance);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.Attendances.AnyAsync(a => a.Id == id, ct);

    public async Task<Attendance?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Attendances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<Attendance?> GetByLessonAndStudentAsync(int lessonId, int studentId, CancellationToken ct = default)
        => await context.Attendances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.LessonId == lessonId && a.StudentId == studentId, ct);

    public async Task<IEnumerable<Attendance>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default)
        => await context.Attendances
            .AsNoTracking()
            .Where(a => a.LessonId == lessonId)
            .Include(a => a.Student)
                .ThenInclude(s => s.User)
            .ToListAsync(ct);

    public async Task<IEnumerable<Attendance>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
        => await context.Attendances
            .AsNoTracking()
            .Where(a => a.StudentId == studentId)
            .Include(a => a.Lesson)
            .OrderBy(a => a.Lesson.LessonDate)
            .ToListAsync(ct);

    public async Task<IEnumerable<Attendance>> GetByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.Attendances
            .AsNoTracking()
            .Where(a => a.StudentId == studentId && a.Lesson.GroupId == groupId)
            .Include(a => a.Lesson)
            .OrderBy(a => a.Lesson.LessonDate)
            .ToListAsync(ct);

    public async Task<IEnumerable<Attendance>> GetByWeekAsync(int groupId, int weekNumber, CancellationToken ct = default)
        => await context.Attendances
            .AsNoTracking()
            .Where(a => a.Lesson.GroupId == groupId && a.Lesson.WeekNumber == weekNumber)
            .Include(a => a.Lesson)
            .Include(a => a.Student)
                .ThenInclude(s => s.User)
            .ToListAsync(ct);

    public async Task<bool> AlreadyExistsAsync(int lessonId, int studentId, CancellationToken ct = default)
        => await context.Attendances
            .AnyAsync(a => a.LessonId == lessonId && a.StudentId == studentId, ct);

    public async Task<int> GetTotalScoreByWeekAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default)
        => await context.Attendances
            .Where(a => a.StudentId == studentId
                     && a.Lesson.GroupId == groupId
                     && a.Lesson.WeekNumber == weekNumber)
            .SumAsync(a => a.Score, ct);

    public async Task<double> GetAverageScoreAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.Attendances
            .Where(a => a.StudentId == studentId && a.Lesson.GroupId == groupId)
            .AverageAsync(a => (double?)a.Score, ct) ?? 0;

    public async Task<int> GetAbsenceCountAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.Attendances
            .CountAsync(a => a.StudentId == studentId
                          && a.Lesson.GroupId == groupId
                          && !a.IsPresent, ct);
}