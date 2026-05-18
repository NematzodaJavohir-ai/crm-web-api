using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudentProgressRepository(DataContext context) : IStudentProgressRepository
{
    public async Task AddAsync(StudentProgress progress, CancellationToken ct = default)
        => await context.StudentProgresses.AddAsync(progress, ct);

    public void Update(StudentProgress progress)
        => context.StudentProgresses.Update(progress);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.StudentProgresses.AnyAsync(sp => sp.Id == id, ct);

    public async Task<StudentProgress?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.StudentProgresses
            .AsNoTracking()
            .Include(sp => sp.Student)
                .ThenInclude(s => s.User)
            .Include(sp => sp.Group)
                .ThenInclude(g => g.Course)
            .FirstOrDefaultAsync(sp => sp.Id == id, ct);

    public async Task<StudentProgress?> GetByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.StudentProgresses
            .AsNoTracking()
            .Include(sp => sp.Student)
                .ThenInclude(s => s.User)
            .Include(sp => sp.Group)
                .ThenInclude(g => g.Course)
            .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.GroupId == groupId, ct);

    public async Task<IEnumerable<StudentProgress>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.StudentProgresses
            .AsNoTracking()
            .Where(sp => sp.GroupId == groupId)
            .Include(sp => sp.Student)
                .ThenInclude(s => s.User)
            .OrderByDescending(sp => sp.AttendanceRate)
            .ToListAsync(ct);

    public async Task<IEnumerable<StudentProgress>> GetRecommendedForCertificateAsync(int groupId, CancellationToken ct = default)
        => await context.StudentProgresses
            .AsNoTracking()
            .Where(sp => sp.GroupId == groupId && sp.IsRecommendedForCertificate)
            .Include(sp => sp.Student)
                .ThenInclude(s => s.User)
            .OrderByDescending(sp => sp.AverageHomeworkScore)
            .ToListAsync(ct);

    public async Task<IEnumerable<StudentProgress>> GetTopByGroupAsync(int groupId, int topCount, CancellationToken ct = default)
        => await context.StudentProgresses
            .AsNoTracking()
            .Where(sp => sp.GroupId == groupId)
            .Include(sp => sp.Student)
                .ThenInclude(s => s.User)
            .OrderByDescending(sp => sp.AverageHomeworkScore)
            .Take(topCount)
            .ToListAsync(ct);
}