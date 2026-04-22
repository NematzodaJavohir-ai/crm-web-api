using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<int> AddUserAsync(User user,CancellationToken cancellationToken=default);
    Task<IEnumerable<User>>GetUsersAsync(CancellationToken cancellationToken=default);
    Task<User?>GetUserByIdAsync(int id,CancellationToken cancellationToken=default);
    Task<bool>DeleteUserAsync(int id,CancellationToken cancellationToken=default);
    Task<bool>UpdateUserAsync(User user,CancellationToken cancellationToken=default);
//----
    Task<bool> EmailExistsAsync(string email,CancellationToken cancellationToken=default);
    Task<bool> PhoneExistsAsync(string phone,CancellationToken cancellationToken=default);
    Task<User?>GetUserByPhoneAsync(string phone,CancellationToken cancellationToken=default);
    Task<User?>GetUserByEmailAsync(string email,CancellationToken cancellationToken=default);
    Task<User?>GetUserByRoleId(int id,CancellationToken ct);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
