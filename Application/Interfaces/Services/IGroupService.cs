using System;
using Application.Dtos.GroupDto;
using Application.Results;
using Domain.Enums;
namespace Application.Interfaces.Services;

public interface IGroupService
{
    // ───── Admin ─────
    Task<Result<GroupResponseDto>> CreateAsync(GroupCreateDto dto, CancellationToken ct = default);
    Task<Result<GroupResponseDto>> UpdateAsync(int id, GroupUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<GroupResponseDto>>> GetAllActiveAsync(CancellationToken ct = default);

    // ───── Admin + Mentor ─────
    Task<Result<GroupResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<GroupWithStudentsDto>> GetWithStudentsAsync(int id, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupResponseDto>>> GetByMentorIdAsync(int mentorId, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupResponseDto>>> GetByCourseIdAsync(int courseId, CancellationToken ct = default);
    Task<Result<GroupResponseDto>> ChangeStatusAsync(int id, GroupStatus status, CancellationToken ct = default);
}