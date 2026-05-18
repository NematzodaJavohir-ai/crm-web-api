using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class HomeworkRepository(DataContext context) : IHomeworkRepository
{
    public async Task AddAsync(Homework homework, CancellationToken ct = default)
        => await context.Homeworks.AddAsync(homework, ct);

    public void Update(Homework homework)
        => context.Homeworks.Update(homework);

    public void Delete(Homework homework)
        => context.Homeworks.Remove(homework);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.Homeworks.AnyAsync(h => h.Id == id, ct);

    

    public async Task<Homework?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Homeworks
            .AsNoTracking()
            .Include(h => h.Lesson)
                .ThenInclude(l => l.Group)
            .FirstOrDefaultAsync(h => h.Id == id, ct);

    public async Task<IEnumerable<Homework>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default)
        => await context.Homeworks
            .AsNoTracking()
            .Where(h => h.LessonId == lessonId)
            .OrderBy(h => h.Deadline)
            .ToListAsync(ct);

    public async Task<IEnumerable<Homework>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.Homeworks
            .AsNoTracking()
            .Where(h => h.Lesson.GroupId == groupId)
            .Include(h => h.Lesson)
            .OrderBy(h => h.Deadline)
            .ToListAsync(ct);

    public async Task<IEnumerable<Homework>> GetByDeadlineAsync(DateTime deadline, CancellationToken ct = default)
        => await context.Homeworks
            .AsNoTracking()
            .Where(h => h.Deadline.Date == deadline.Date)
            .Include(h => h.Lesson)
                .ThenInclude(l => l.Group)
            .OrderBy(h => h.Deadline)
            .ToListAsync(ct);

    public async Task<IEnumerable<Homework>> GetOverdueAsync(CancellationToken ct = default)
        => await context.Homeworks
            .AsNoTracking()
            .Where(h => h.Deadline < DateTime.UtcNow)
            .Include(h => h.Lesson)
                .ThenInclude(l => l.Group)
            .OrderBy(h => h.Deadline)
            .ToListAsync(ct);
}