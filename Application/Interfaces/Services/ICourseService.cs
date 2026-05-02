using System;
using Application.Dtos.CourseDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface ICourseService
{
    // ───── Admin ─────
    Task<Result<CourseResponseDto>> CreateAsync(CourseCreateDto dto, CancellationToken ct = default);
    Task<Result<CourseResponseDto>> UpdateAsync(int id, CourseUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<CourseResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default);

    // ───── Any Role ─────
    Task<Result<IEnumerable<CourseResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<CourseResponseDto>>> GetAllActiveAsync(CancellationToken ct = default);
    Task<Result<CourseResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<CourseWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default);
}
