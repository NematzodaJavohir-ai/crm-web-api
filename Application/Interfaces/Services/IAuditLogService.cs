using System;
using Application.Dtos.AuditLogDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IAuditLogService
{
    Task<Result<AuditLogResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<IEnumerable<AuditLogResponseDto>>> GetByEntityAsync(string entityName, int entityId, CancellationToken ct = default);
    Task<Result<IEnumerable<AuditLogResponseDto>>> GetByUserAsync(int userId, CancellationToken ct = default);
    Task<Result<IEnumerable<AuditLogResponseDto>>> GetByActionAsync(string action, CancellationToken ct = default);
    Task<Result<IEnumerable<AuditLogResponseDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<Result<IEnumerable<AuditLogResponseDto>>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Result<int>> GetTotalCountAsync(CancellationToken ct = default);
}

