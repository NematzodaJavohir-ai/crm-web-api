using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProfileRepository(DataContext context) : IProfileRepository
{
    public async Task AddAsync(Profile profile, CancellationToken ct = default)
        => await context.Profiles.AddAsync(profile, ct);

    public void Update(Profile profile)
        => context.Profiles.Update(profile);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.Profiles.AnyAsync(p => p.Id == id, ct);

    public async Task<Profile?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Profiles
            .AsNoTracking()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Profile?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await context.Profiles
            .AsNoTracking()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

    public async Task<IEnumerable<Profile>> SearchByNameAsync(string searchTerm, CancellationToken ct = default)
        => await context.Profiles
            .AsNoTracking()
            .Where(p => (p.FirstName != null && p.FirstName.Contains(searchTerm))
                        || (p.LastName != null && p.LastName.Contains(searchTerm)))
            .Include(p => p.User)
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName)
            .ToListAsync(ct);

    public async Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default)
        => await context.Profiles.AnyAsync(p => p.Phone == phone, ct);
}