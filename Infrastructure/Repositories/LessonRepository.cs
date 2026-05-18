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

    public async Task<int> GetCompletedCountAsync(int groupId, CancellationToken ct = default)
        => await context.Lessons
            .CountAsync(l => l.GroupId == groupId && l.IsCompleted, ct);

    public async Task<Lesson?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Lessons
            .AsNoTracking()
            .Include(l => l.Group)
                .ThenInclude(g => g.Course)
            .Include(l => l.Group)
                .ThenInclude(g => g.Mentor)
                    .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<Lesson?> GetWithAttendanceAsync(int id, CancellationToken ct = default)
        => await context.Lessons
            .AsNoTracking()
            .Include(l => l.Group)
                .ThenInclude(g => g.Course)
            .Include(l => l.Group)
                .ThenInclude(g => g.Mentor)
                    .ThenInclude(m => m.User)
            .Include(l => l.Attendances.OrderBy(a => a.Student.User.FirstName))
                .ThenInclude(a => a.Student)
                    .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<Lesson?> GetNextLessonAsync(int groupId, CancellationToken ct = default)
        => await context.Lessons
            .AsNoTracking()
            .Where(l => l.GroupId == groupId
                     && !l.IsCompleted
                     && l.LessonDate >= DateTime.UtcNow.Date)
            .Include(l => l.Group)
                .ThenInclude(g => g.Course)
            .Include(l => l.Group)
                .ThenInclude(g => g.Mentor)
                    .ThenInclude(m => m.User)
            .OrderBy(l => l.LessonDate)
            .FirstOrDefaultAsync(ct);

    public async Task<IEnumerable<Lesson>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.Lessons
            .AsNoTracking()
            .Where(l => l.GroupId == groupId)
            .Include(l => l.Group)
                .ThenInclude(g => g.Course)
            .Include(l => l.Group)
                .ThenInclude(g => g.Mentor)
                    .ThenInclude(m => m.User)
            .OrderBy(l => l.LessonDate)
            .ToListAsync(ct);
public async Task<IEnumerable<Lesson>> GetAllAsync(CancellationToken ct = default)
    => await context.Lessons
        .AsNoTracking()
        .Include(l => l.Group)
        .OrderByDescending(l => l.LessonDate)
        .ToListAsync(ct);

public async Task<bool> DateExistsInGroupAsync(int groupId, DateTime date, CancellationToken ct = default)
    => await context.Lessons
        .AnyAsync(l => l.GroupId == groupId && l.LessonDate.Date == date.Date, ct);
    public async Task<IEnumerable<Lesson>> GetByWeekAsync(int groupId, int weekNumber, CancellationToken ct = default)
        => await context.Lessons
            .AsNoTracking()
            .Where(l => l.GroupId == groupId && l.WeekNumber == weekNumber)
            .Include(l => l.Group)
                .ThenInclude(g => g.Course)
            .Include(l => l.Attendances)
                .ThenInclude(a => a.Student)
                    .ThenInclude(s => s.User)
            .OrderBy(l => l.LessonDate)
            .ToListAsync(ct);

    public async Task<IEnumerable<Lesson>> GetByDateRangeAsync(int groupId, DateTime from, DateTime to, CancellationToken ct = default)
        => await context.Lessons
            .AsNoTracking()
            .Where(l => l.GroupId == groupId
                     && l.LessonDate.Date >= from.Date
                     && l.LessonDate.Date <= to.Date)
            .Include(l => l.Group)
                .ThenInclude(g => g.Course)
            .Include(l => l.Group)
                .ThenInclude(g => g.Mentor)
                    .ThenInclude(m => m.User)
            .OrderBy(l => l.LessonDate)
            .ToListAsync(ct);
}