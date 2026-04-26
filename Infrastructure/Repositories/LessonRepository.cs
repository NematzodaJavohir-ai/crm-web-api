using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LessonRepository(DataContext context) : ILessonRepository
{
    public async Task AddAsync(Lesson lesson, CancellationToken ct = default)
        => await context.Lessons.AddAsync(lesson, ct);

    public void Update(Lesson lesson)
        => context.Lessons.Update(lesson);

    public void Delete(Lesson lesson)
        => context.Lessons.Remove(lesson);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.Lessons.AnyAsync(l => l.Id == id, ct);

    public async Task<Lesson?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<Lesson?> GetWithAttendancesAsync(int id, CancellationToken ct = default)
        => await context.Lessons
            .AsNoTracking()
            .Include(l => l.Attendances)
                .ThenInclude(a => a.Student)
                    .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<IEnumerable<Lesson>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.Lessons
            .AsNoTracking()
            .Where(l => l.GroupId == groupId)
            .OrderBy(l => l.LessonDate)
            .ToListAsync(ct);

    public async Task<IEnumerable<Lesson>> GetByWeekNumberAsync(int groupId, int weekNumber, CancellationToken ct = default)
        => await context.Lessons
            .AsNoTracking()
            .Where(l => l.GroupId == groupId && l.WeekNumber == weekNumber)
            .OrderBy(l => l.LessonDate)
            .Include(l => l.Attendances)
                .ThenInclude(a => a.Student)
                    .ThenInclude(s => s.User)
            .ToListAsync(ct);

    public async Task<Lesson?> GetByGroupAndDateAsync(int groupId, DateTime date, CancellationToken ct = default)
        => await context.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.GroupId == groupId && l.LessonDate.Date == date.Date, ct);

    public async Task<bool> DateExistsInGroupAsync(int groupId, DateTime date, CancellationToken ct = default)
        => await context.Lessons
            .AnyAsync(l => l.GroupId == groupId && l.LessonDate.Date == date.Date, ct);

    public async Task<int> GetLastWeekNumberAsync(int groupId, CancellationToken ct = default)
        => await context.Lessons
            .Where(l => l.GroupId == groupId)
            .MaxAsync(l => (int?)l.WeekNumber, ct) ?? 0;
}
