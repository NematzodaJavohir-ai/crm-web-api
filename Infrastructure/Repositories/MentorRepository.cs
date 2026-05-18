using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MentorRepository(DataContext context) : IMentorRepository
{
    public async Task AddAsync(Mentor mentor, CancellationToken ct = default)
        => await context.Mentors.AddAsync(mentor, ct);

    public void Update(Mentor mentor)
        => context.Mentors.Update(mentor);

    public void Delete(Mentor mentor)
        => context.Mentors.Remove(mentor);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.Mentors.AnyAsync(m => m.Id == id, ct);

    public async Task<int> GetActiveGroupCountAsync(int id, CancellationToken ct = default)
        => await context.Groups
            .CountAsync(g => g.MentorId == id && g.Status == GroupStatus.Active, ct);

    public async Task<Mentor?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .Include(m => m.User)
                .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<Mentor?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .Include(m => m.User)
                .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(m => m.UserId == userId, ct);

    public async Task<Mentor?> GetWithGroupsAsync(int id, CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .Include(m => m.User)
                .ThenInclude(u => u.Role)
            .Include(m => m.Groups.OrderByDescending(g => g.CreatedAt))
                .ThenInclude(g => g.Course)
            .Include(m => m.Groups)
                .ThenInclude(g => g.GroupStudents.Where(gs => gs.IsActive))
                    .ThenInclude(gs => gs.Student)
                        .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<IEnumerable<Mentor>> GetAllAsync(CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .Include(m => m.User)
                .ThenInclude(u => u.Role)
            .Include(m => m.Groups.Where(g => g.Status == GroupStatus.Active))
                .ThenInclude(g => g.Course)
            .OrderBy(m => m.User.FirstName)
                .ThenBy(m => m.User.LastName)
            .ToListAsync(ct);

    public async Task<IEnumerable<Mentor>> GetActiveAsync(CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .Where(m => m.IsActive)
            .Include(m => m.User)
                .ThenInclude(u => u.Role)
            .Include(m => m.Groups.Where(g => g.Status == GroupStatus.Active))
                .ThenInclude(g => g.Course)
            .OrderBy(m => m.User.FirstName)
                .ThenBy(m => m.User.LastName)
            .ToListAsync(ct);

    public async Task<IEnumerable<Mentor>> GetBySpecializationAsync(string specialization, CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .Where(m => m.IsActive &&
                        m.Specialization != null &&
                        m.Specialization.ToLower().Contains(specialization.ToLower()))
            .Include(m => m.User)
                .ThenInclude(u => u.Role)
            .Include(m => m.Groups.Where(g => g.Status == GroupStatus.Active))
                .ThenInclude(g => g.Course)
            .OrderBy(m => m.User.FirstName)
                .ThenBy(m => m.User.LastName)
            .ToListAsync(ct);
}