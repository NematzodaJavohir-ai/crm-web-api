using Application.Interfaces.Repositories;
using Domain.Entities;
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

    public async Task<Mentor?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<Mentor?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.UserId == userId, ct);

    public async Task<Mentor?> GetWithUserAsync(int id, CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<Mentor?> GetWithGroupsAsync(int id, CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .Include(m => m.Groups)
                .ThenInclude(g => g.Course)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<Mentor?> GetFullProfileAsync(int id, CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .Include(m => m.User)
            .Include(m => m.Groups)
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
            .ToListAsync(ct);

    public async Task<IEnumerable<Mentor>> GetAllActiveAsync(CancellationToken ct = default)
        => await context.Mentors
            .AsNoTracking()
            .Where(m => m.IsActive)
            .Include(m => m.User)
            .ToListAsync(ct);
}
