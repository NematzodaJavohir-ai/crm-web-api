using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IStudentProgressRepository
{
    Task AddAsync(StudentProgress progress, CancellationToken ct = default);
    void Update(StudentProgress progress);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    

    Task<StudentProgress?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<StudentProgress?> GetByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default);
    Task<IEnumerable<StudentProgress>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<IEnumerable<StudentProgress>> GetRecommendedForCertificateAsync(int groupId, CancellationToken ct = default);
    Task<IEnumerable<StudentProgress>> GetTopByGroupAsync(int groupId, int topCount, CancellationToken ct = default);
}