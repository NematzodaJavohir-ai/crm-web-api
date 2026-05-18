using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SheduleRepository(DataContext context) : ISheduleRepository
{
    public async Task AddAsync(Shedule shedule, CancellationToken ct = default)
        => await context.Shedules.AddAsync(shedule, ct);

    public void Update(Shedule shedule)
        => context.Shedules.Update(shedule);

    public void Delete(Shedule shedule)
        => context.Shedules.Remove(shedule);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.Shedules.AnyAsync(s => s.Id == id, ct);

    public async Task<Shedule?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Shedules
            .AsNoTracking()
            .Include(s => s.Group)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IEnumerable<Shedule>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.Shedules
            .AsNoTracking()
            .Where(s => s.GroupId == groupId)
            .Include(s => s.Group)
            .OrderBy(s => s.Day)
            .ThenBy(s => s.StartTime)
            .ToListAsync(ct);

    public async Task<IEnumerable<Shedule>> GetByDayAsync(DayOfWeek day, CancellationToken ct = default)
        => await context.Shedules
            .AsNoTracking()
            .Where(s => s.Day == day)
            .Include(s => s.Group)
            .OrderBy(s => s.StartTime)
            .ToListAsync(ct);

    public async Task<bool> HasConflictAsync(int groupId, DayOfWeek day, TimeSpan start, TimeSpan end, CancellationToken ct = default)
        => await context.Shedules
            .AnyAsync(s => s.GroupId == groupId
                           && s.Day == day
                           && s.StartTime < end
                           && s.EndTime > start, ct);
}