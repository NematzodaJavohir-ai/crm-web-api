using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GroupRepository(DataContext context) : IGroupRepository
{
    public async Task AddAsync(Group group, CancellationToken ct = default)
        => await context.Groups.AddAsync(group, ct);

    public void Update(Group group)
        => context.Groups.Update(group);

    public void Delete(Group group)
        => context.Groups.Remove(group);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.Groups.AnyAsync(g => g.Id == id, ct);

    public async Task<Group?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<Group?> GetWithDetailsAsync(int id, CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Include(g => g.Course)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .Include(g => g.GroupStudents.Where(gs => gs.IsActive))
                .ThenInclude(gs => gs.Student)
                    .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<IEnumerable<Group>> GetAllAsync(CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Include(g => g.Course)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .ToListAsync(ct);

    public async Task<IEnumerable<Group>> GetAllActiveAsync(CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Where(g => g.Status == GroupStatus.Active)
            .Include(g => g.Course)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .ToListAsync(ct);

    public async Task<IEnumerable<Group>> GetByMentorIdAsync(int mentorId, CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Where(g => g.MentorId == mentorId)
            .Include(g => g.Course)
            .Include(g => g.GroupStudents.Where(gs => gs.IsActive))
                .ThenInclude(gs => gs.Student)
                    .ThenInclude(s => s.User)
            .ToListAsync(ct);

    public async Task<IEnumerable<Group>> GetByCourseIdAsync(int courseId, CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Where(g => g.CourseId == courseId)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .ToListAsync(ct);

    public async Task<int> GetStudentCountAsync(int groupId, CancellationToken ct = default)
        => await context.GroupStudents
            .CountAsync(gs => gs.GroupId == groupId && gs.IsActive, ct);
}