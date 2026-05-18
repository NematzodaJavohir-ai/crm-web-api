using Application.Dtos.AuditLogDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Enums;

namespace Application.Services;

public class AuditLogService(IUnitOfWork uow) : IAuditLogService
{
    public async Task<Result<AuditLogResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var log = await uow.AuditLogs.GetByIdAsync(id, ct);
        if (log is null)
            return Result<AuditLogResponseDto>.Fail("Audit log not found", ErrorType.NotFound);

        return Result<AuditLogResponseDto>.Ok(MapToResponseDto(log));
    }

    public async Task<Result<IEnumerable<AuditLogResponseDto>>> GetByEntityAsync(string entityName, int entityId, CancellationToken ct = default)
    {
        var logs = await uow.AuditLogs.GetByEntityAsync(entityName, entityId, ct);
        return Result<IEnumerable<AuditLogResponseDto>>.Ok(logs.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<AuditLogResponseDto>>> GetByUserAsync(int userId, CancellationToken ct = default)
    {
        var logs = await uow.AuditLogs.GetByUserAsync(userId, ct);
        return Result<IEnumerable<AuditLogResponseDto>>.Ok(logs.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<AuditLogResponseDto>>> GetByActionAsync(string action, CancellationToken ct = default)
    {
        var logs = await uow.AuditLogs.GetByActionAsync(action, ct);
        return Result<IEnumerable<AuditLogResponseDto>>.Ok(logs.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<AuditLogResponseDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var logs = await uow.AuditLogs.GetByDateRangeAsync(from, to, ct);
        return Result<IEnumerable<AuditLogResponseDto>>.Ok(logs.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<AuditLogResponseDto>>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var logs = await uow.AuditLogs.GetAllAsync(page, pageSize, ct);
        return Result<IEnumerable<AuditLogResponseDto>>.Ok(logs.Select(MapToResponseDto));
    }

    public async Task<Result<int>> GetTotalCountAsync(CancellationToken ct = default)
    {
        var count = await uow.AuditLogs.GetTotalCountAsync(ct);
        return Result<int>.Ok(count);
    }

    private static AuditLogResponseDto MapToResponseDto(Domain.Entities.AuditLog log) => new()
    {
        Id = log.Id,
        UserId = log.UserId,
        EntityName = log.EntityName,
        Action = log.Action,
        OldValues = log.OldValues,
        NewValues = log.NewValues,
        Timestamp = log.Timestamp,
        IpAddress = log.IpAddress
    };
}