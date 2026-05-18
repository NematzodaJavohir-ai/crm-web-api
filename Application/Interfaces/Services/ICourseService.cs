using System;
using Application.Dtos.CourseDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface ICourseService
{
    // ───── Basic CRUD ─────
    Task<Result<CourseResponseDto>> CreateAsync(CourseCreateDto dto, CancellationToken ct = default);
    Task<Result<CourseResponseDto>> UpdateAsync(int id, CourseUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<CourseResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);

    // ───── Queries ─────
    Task<Result<IEnumerable<CourseResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<CourseResponseDto>>> GetAllActiveAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<CourseResponseDto>>> GetByPriceRangeAsync(decimal min, decimal max, CancellationToken ct = default);

    // ───── Detailed ─────
    Task<Result<CourseWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default);
    Task<Result<CourseWithGroupsDto>> GetWithActiveGroupsAsync(int id, CancellationToken ct = default);

    // ───── Admin Actions ─────
    Task<Result<CourseResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default);
    Task<Result<bool>> NameExistsAsync(string name, CancellationToken ct = default);
    Task<Result<int>> GetGroupCountAsync(int id, CancellationToken ct = default);

    // ───── Lookup (for modals/dropdowns) ─────
    Task<Result<IEnumerable<CourseShortDto>>> GetLookupAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<CourseShortDto>>> GetActiveLookupAsync(CancellationToken ct = default);
}