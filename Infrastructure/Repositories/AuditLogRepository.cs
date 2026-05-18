using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AuditLogRepository(DataContext context) : IAuditLogRepository
{
    public async Task AddAsync(AuditLog auditLog, CancellationToken ct = default)
        => await context.AuditLogs.AddAsync(auditLog, ct);

    public async Task<AuditLog?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.AuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, int entityId, CancellationToken ct = default)
        => await context.AuditLogs
            .AsNoTracking()
            .Where(a => a.EntityName == entityName && a.OldValues.Contains(entityId.ToString()))
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(ct);

    public async Task<IEnumerable<AuditLog>> GetByUserAsync(int userId, CancellationToken ct = default)
        => await context.AuditLogs
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(ct);

    public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action, CancellationToken ct = default)
        => await context.AuditLogs
            .AsNoTracking()
            .Where(a => a.Action == action)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(ct);

    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await context.AuditLogs
            .AsNoTracking()
            .Where(a => a.Timestamp >= from && a.Timestamp <= to)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(ct);

    public async Task<IEnumerable<AuditLog>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        => await context.AuditLogs
            .AsNoTracking()
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
        => await context.AuditLogs.CountAsync(ct);
}