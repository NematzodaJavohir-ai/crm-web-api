using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICourseRepository
{
    Task AddAsync(Course course, CancellationToken ct = default);
    void Update(Course course);
    void Delete(Course course);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    Task<Course?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Course?> GetWithGroupsAsync(int id, CancellationToken ct = default);          
    Task<IEnumerable<Course>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Course>> GetAllActiveAsync(CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);  
}
