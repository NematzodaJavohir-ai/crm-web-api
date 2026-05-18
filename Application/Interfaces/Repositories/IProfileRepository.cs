using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IProfileRepository
{
    Task AddAsync(Profile profile, CancellationToken ct = default);
    void Update(Profile profile);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    

    Task<Profile?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Profile?> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<IEnumerable<Profile>> SearchByNameAsync(string searchTerm, CancellationToken ct = default);
    Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default);
}