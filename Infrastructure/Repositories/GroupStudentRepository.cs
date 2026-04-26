using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GroupStudentRepository(DataContext context) : IGroupStudentRepository
{
    public async Task AddAsync(GroupStudent groupStudent, CancellationToken ct = default)
        => await context.GroupStudents.AddAsync(groupStudent, ct);

    public void Update(GroupStudent groupStudent)
        => context.GroupStudents.Update(groupStudent);

    public void Delete(GroupStudent groupStudent)
        => context.GroupStudents.Remove(groupStudent);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.GroupStudents.AnyAsync(gs => gs.Id == id, ct);

    public async Task<GroupStudent?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.GroupStudents
            .AsNoTracking()
            .FirstOrDefaultAsync(gs => gs.Id == id, ct);

    public async Task<GroupStudent?> GetByGroupAndStudentAsync(int groupId, int studentId, CancellationToken ct = default)
        => await context.GroupStudents
            .AsNoTracking()
            .FirstOrDefaultAsync(gs => gs.GroupId == groupId && gs.StudentId == studentId, ct);

    public async Task<IEnumerable<GroupStudent>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.GroupStudents
            .AsNoTracking()
            .Where(gs => gs.GroupId == groupId)
            .Include(gs => gs.Student)
                .ThenInclude(s => s.User)
            .ToListAsync(ct);

    public async Task<IEnumerable<GroupStudent>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
        => await context.GroupStudents
            .AsNoTracking()
            .Where(gs => gs.StudentId == studentId)
            .Include(gs => gs.Group)
                .ThenInclude(g => g.Course)
            .ToListAsync(ct);

    public async Task<IEnumerable<GroupStudent>> GetActiveByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.GroupStudents
            .AsNoTracking()
            .Where(gs => gs.GroupId == groupId && gs.IsActive)
            .Include(gs => gs.Student)
                .ThenInclude(s => s.User)
            .ToListAsync(ct);

    public async Task<bool> IsStudentInGroupAsync(int groupId, int studentId, CancellationToken ct = default)
        => await context.GroupStudents
            .AnyAsync(gs => gs.GroupId == groupId && gs.StudentId == studentId && gs.IsActive, ct);

    public async Task RemoveStudentAsync(int groupId, int studentId, string? reason, CancellationToken ct = default)
    {
        var groupStudent = await context.GroupStudents
            .FirstOrDefaultAsync(gs => gs.GroupId == groupId && gs.StudentId == studentId, ct);

        if (groupStudent is null) return;

        groupStudent.IsActive = false;
        groupStudent.LeftAt = DateTime.UtcNow;
        groupStudent.RemoveReason = reason;

        context.GroupStudents.Update(groupStudent);
    }

    public async Task<int> GetActiveStudentCountAsync(int groupId, CancellationToken ct = default)
        => await context.GroupStudents
            .CountAsync(gs => gs.GroupId == groupId && gs.IsActive, ct);
}