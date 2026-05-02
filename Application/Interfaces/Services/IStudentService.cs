using System;
using Application.Dtos.StudentDto;
using Application.Results;

namespace Application.Interfaces.Services;
public interface IStudentService
{
    // ───── Admin ─────
    Task<Result<StudentResponseDto>> CreateAsync(StudentCreateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<IEnumerable<StudentResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<StudentResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default);

    // ───── Admin + Mentor ─────
    Task<Result<IEnumerable<StudentResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<Result<StudentWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default);
    Task<Result<StudentFullProfileDto>> GetFullProfileAsync(int id, CancellationToken ct = default);

    // ───── Student (свой профиль) ─────
    Task<Result<StudentResponseDto>> GetMyProfileAsync(int userId, CancellationToken ct = default);
    Task<Result<StudentResponseDto>> UpdateMyProfileAsync(int userId, StudentUpdateDto dto, CancellationToken ct = default);
    Task<Result<StudentFullProfileDto>> GetMyFullProfileAsync(int userId, CancellationToken ct = default);
}