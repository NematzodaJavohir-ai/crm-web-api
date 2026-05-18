using Domain.Entities;

namespace Application.Interfaces.Repositories;



public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken ct = default);
    Task<AuditLog?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, int entityId, CancellationToken ct = default);
    Task<IEnumerable<AuditLog>> GetByUserAsync(int userId, CancellationToken ct = default);
    Task<IEnumerable<AuditLog>> GetByActionAsync(string action, CancellationToken ct = default);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<IEnumerable<AuditLog>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
}