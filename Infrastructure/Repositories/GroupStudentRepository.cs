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

    public async Task<bool> IsStudentInGroupAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.GroupStudents
            .AnyAsync(gs => gs.StudentId == studentId && gs.GroupId == groupId && gs.IsActive, ct);

    public async Task<int> GetActiveStudentCountAsync(int groupId, CancellationToken ct = default)
        => await context.GroupStudents
            .CountAsync(gs => gs.GroupId == groupId && gs.IsActive, ct);

    public async Task<GroupStudent?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.GroupStudents
            .AsNoTracking()
            .Include(gs => gs.Student)
                .ThenInclude(s => s.User)
            .Include(gs => gs.Group)
                .ThenInclude(g => g.Course)
            .Include(gs => gs.Group)
                .ThenInclude(g => g.Mentor)
                    .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(gs => gs.Id == id, ct);

    public async Task<GroupStudent?> GetActiveByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.GroupStudents
            .AsNoTracking()
            .Include(gs => gs.Student)
                .ThenInclude(s => s.User)
            .Include(gs => gs.Group)
                .ThenInclude(g => g.Course)
            .FirstOrDefaultAsync(gs => gs.StudentId == studentId
                                    && gs.GroupId == groupId
                                    && gs.IsActive, ct);

    public async Task<IEnumerable<GroupStudent>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.GroupStudents
            .AsNoTracking()
            .Where(gs => gs.GroupId == groupId)
            .Include(gs => gs.Student)
                .ThenInclude(s => s.User)
            .Include(gs => gs.Group)
                .ThenInclude(g => g.Course)
            .OrderBy(gs => gs.Student.User.FirstName)
                .ThenBy(gs => gs.Student.User.LastName)
            .ToListAsync(ct);

    public async Task<IEnumerable<GroupStudent>> GetActiveByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.GroupStudents
            .AsNoTracking()
            .Where(gs => gs.GroupId == groupId && gs.IsActive)
            .Include(gs => gs.Student)
                .ThenInclude(s => s.User)
            .Include(gs => gs.Group)
                .ThenInclude(g => g.Course)
            .OrderBy(gs => gs.Student.User.FirstName)
                .ThenBy(gs => gs.Student.User.LastName)
            .ToListAsync(ct);

    public async Task<IEnumerable<GroupStudent>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
        => await context.GroupStudents
            .AsNoTracking()
            .Where(gs => gs.StudentId == studentId)
            .Include(gs => gs.Group)
                .ThenInclude(g => g.Course)
            .Include(gs => gs.Group)
                .ThenInclude(g => g.Mentor)
                    .ThenInclude(m => m.User)
            .OrderByDescending(gs => gs.JoinedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<GroupStudent>> GetTransferHistoryAsync(int studentId, CancellationToken ct = default)
        => await context.GroupStudents
            .AsNoTracking()
            .Where(gs => gs.StudentId == studentId 
                         && (gs.TransferredFromGroupStudentId != null || gs.TransferredToGroupStudentId != null))
            .Include(gs => gs.Group)
            .ThenInclude(g => g.Course)
            .Include(gs => gs.TransferredFrom)
            .ThenInclude(tf => tf.Group)
            .Include(gs => gs.TransferredTo)
            .ThenInclude(tt => tt.Group)
            .OrderByDescending(gs => gs.JoinedAt)
            .ToListAsync(ct);

    public async Task RemoveStudentAsync(int groupId, int studentId, string? reason, CancellationToken ct = default)
    {
        var groupStudent = await context.GroupStudents
            .FirstOrDefaultAsync(gs => gs.GroupId == groupId
                                    && gs.StudentId == studentId
                                    && gs.IsActive, ct);

        if (groupStudent is null) return;

        groupStudent.IsActive = false;
        groupStudent.LeftAt = DateTime.UtcNow;
        groupStudent.RemoveReason = reason;

        context.GroupStudents.Update(groupStudent);
    }
}