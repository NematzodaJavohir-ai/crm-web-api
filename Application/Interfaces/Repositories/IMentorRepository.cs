using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IMentorRepository
{
    Task AddAsync(Mentor mentor, CancellationToken ct = default);
    void Update(Mentor mentor);
    void Delete(Mentor mentor);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);


    Task<Mentor?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Mentor?> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<Mentor?> GetWithGroupsAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Mentor>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Mentor>> GetActiveAsync(CancellationToken ct = default);
    Task<IEnumerable<Mentor>> GetBySpecializationAsync(string specialization, CancellationToken ct = default);
    Task<int> GetActiveGroupCountAsync(int id, CancellationToken ct = default);
}