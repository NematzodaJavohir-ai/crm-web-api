using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(DataContext context) : IUserRepository
{
    public async Task<int> AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
        return user.Id;
    }

    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
        if (user == null) return false;
        
        context.Users.Remove(user);
        return true;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var cleanEmail = email.Trim().ToLower();
        return await context.Users.AnyAsync(user => user.Email == cleanEmail, cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .Include(us => us.Role)
            .FirstOrDefaultAsync(user => user.Email == email.Trim().ToLower(), cancellationToken);
    }

    public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
             .Include( us => us.Role)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetUserByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .Include(us => us.Role)
            .FirstOrDefaultAsync(user => user.PhoneNumber == phone.Trim(), cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await context.Users.AsNoTracking()
                                  .Include(us => us.Role)
                                  .ToListAsync(cancellationToken);
    }

    public async Task<bool> PhoneExistsAsync(string phone, CancellationToken cancellationToken = default)
    {
        var cleanPhone = phone.Trim();
        return await context.Users.AnyAsync(user => user.PhoneNumber == cleanPhone, cancellationToken);
    }

    public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        return await Task.FromResult(true);
    }

    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetUserByRoleId(int id, CancellationToken ct)
    {
        return await context.Users.AsNoTracking()
                                   .FirstOrDefaultAsync(us =>us.Id==id,ct);
                                  
    }

}