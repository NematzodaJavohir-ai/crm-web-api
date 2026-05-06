using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IStudentRepository
{
    Task AddAsync(Student student, CancellationToken ct = default);
    void Update(Student student);
    void Delete(Student student);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    Task<Student?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Student?> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<Student?> GetWithUserAsync(int id, CancellationToken ct = default);          
    Task<Student?> GetWithGroupsAsync(int id, CancellationToken ct = default);        
    Task<Student?> GetWithAttendancesAsync(int id, CancellationToken ct = default);  
    Task<Student?> GetWithWeekResultsAsync(int id, CancellationToken ct = default);    
    Task<Student?> GetFullProfileAsync(int id, CancellationToken ct = default);        
    Task<IEnumerable<Student>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Student>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    
}
