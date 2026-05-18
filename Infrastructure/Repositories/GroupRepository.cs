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

    public async Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => await context.Groups.AnyAsync(g => g.Name.ToLower() == name.ToLower(), ct);

    public async Task<bool> HasAvailableSeatsAsync(int id, CancellationToken ct = default)
    {
        var group = await context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id, ct);

        if (group is null) return false;

        var activeCount = await context.GroupStudents
            .CountAsync(gs => gs.GroupId == id && gs.IsActive, ct);

        return activeCount < group.MaxStudents;
    }

    public async Task<int> GetStudentCountAsync(int id, CancellationToken ct = default)
        => await context.GroupStudents
            .CountAsync(gs => gs.GroupId == id && gs.IsActive, ct);

    public async Task<Group?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Include(g => g.Course)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<Group?> GetWithStudentsAsync(int id, CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Include(g => g.Course)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .Include(g => g.GroupStudents.Where(gs => gs.IsActive))
                .ThenInclude(gs => gs.Student)
                    .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<Group?> GetWithLessonsAsync(int id, CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Include(g => g.Course)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .Include(g => g.Lessons.OrderBy(l => l.LessonDate))
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<IEnumerable<Group>> GetAllAsync(CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Include(g => g.Course)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Group>> GetActiveAsync(CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Where(g => g.Status == GroupStatus.Active)
            .Include(g => g.Course)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .Include(g => g.GroupStudents.Where(gs => gs.IsActive))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Group>> GetByStatusAsync(GroupStatus status, CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Where(g => g.Status == status)
            .Include(g => g.Course)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Group>> GetByCourseIdAsync(int courseId, CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Where(g => g.CourseId == courseId)
            .Include(g => g.Course)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .Include(g => g.GroupStudents.Where(gs => gs.IsActive))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Group>> GetByMentorIdAsync(int mentorId, CancellationToken ct = default)
        => await context.Groups
            .AsNoTracking()
            .Where(g => g.MentorId == mentorId)
            .Include(g => g.Course)
            .Include(g => g.Mentor)
                .ThenInclude(m => m.User)
            .Include(g => g.GroupStudents.Where(gs => gs.IsActive))
                .ThenInclude(gs => gs.Student)
                    .ThenInclude(s => s.User)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);
}