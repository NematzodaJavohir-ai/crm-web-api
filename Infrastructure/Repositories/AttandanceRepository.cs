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
            .ToListAsync(ct);

    public async Task<IEnumerable<Attendance>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
        => await context.Attendances
            .AsNoTracking()
            .Where(a => a.StudentId == studentId)
            .ToListAsync(ct);

    public async Task<IEnumerable<Attendance>> GetByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.Attendances
            .AsNoTracking()
            .Where(a => a.StudentId == studentId && a.Lesson.GroupId == groupId)
            .ToListAsync(ct);

    public async Task<IEnumerable<Attendance>> GetByWeekAsync(int groupId, int weekNumber, CancellationToken ct = default)
        => await context.Attendances
            .AsNoTracking()
            .Where(a => a.Lesson.GroupId == groupId && a.Lesson.WeekNumber == weekNumber)
            .ToListAsync(ct);

    public async Task<bool> AlreadyExistsAsync(int lessonId, int studentId, CancellationToken ct = default)
        => await context.Attendances
            .AnyAsync(a => a.LessonId == lessonId && a.StudentId == studentId, ct);

    public async Task<int> GetPresentCountByWeekAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default)
        => await context.Attendances
            .AsNoTracking()
            .CountAsync(a => a.StudentId == studentId 
                && a.Lesson.GroupId == groupId 
                && a.Lesson.WeekNumber == weekNumber 
                && a.IsPresent, ct);

    public async Task<int> GetAbsenceCountAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.Attendances
            .AsNoTracking()
            .CountAsync(a => a.StudentId == studentId 
                && a.Lesson.GroupId == groupId 
                && !a.IsPresent, ct);

    public async Task<double> GetAttendanceRateAsync(int studentId, int groupId, CancellationToken ct = default)
    {
        var total = await context.Attendances
            .AsNoTracking()
            .CountAsync(a => a.StudentId == studentId && a.Lesson.GroupId == groupId, ct);

        if (total == 0)
            return 0;

        var present = await context.Attendances
            .AsNoTracking()
            .CountAsync(a => a.StudentId == studentId && a.Lesson.GroupId == groupId && a.IsPresent, ct);

        return (double)present / total * 100;
    }
}