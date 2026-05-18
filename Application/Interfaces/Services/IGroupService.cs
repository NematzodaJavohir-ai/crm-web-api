using System;
using Application.Dtos.GroupDto;
using Application.Results;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IGroupService
{
    // ───── Basic CRUD ─────
    Task<Result<GroupResponseDto>> CreateAsync(GroupCreateDto dto, CancellationToken ct = default);
    Task<Result<GroupResponseDto>> UpdateAsync(int id, GroupUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<GroupResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);

    // ───── Queries ─────
    Task<Result<IEnumerable<GroupResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<GroupResponseDto>>> GetActiveAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<GroupResponseDto>>> GetByStatusAsync(GroupStatus status, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupResponseDto>>> GetByCourseIdAsync(int courseId, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupResponseDto>>> GetByMentorIdAsync(int mentorId, CancellationToken ct = default);

    // ───── Detailed ─────
    Task<Result<GroupWithStudentsDto>> GetWithStudentsAsync(int id, CancellationToken ct = default);
    Task<Result<GroupWithLessonsDto>> GetWithLessonsAsync(int id, CancellationToken ct = default);
    Task<Result<GroupFullDto>> GetFullAsync(int id, CancellationToken ct = default);

    // ───── Admin Actions ─────
    Task<Result<GroupResponseDto>> ChangeStatusAsync(int id, GroupStatus status, CancellationToken ct = default);
    Task<Result<GroupResponseDto>> ChangeMentorAsync(int id, int mentorId, CancellationToken ct = default);
    Task<Result<int>> GetActiveStudentCountAsync(int id, CancellationToken ct = default);


    // ───── Lookup (modals/dropdowns) ─────
    Task<Result<IEnumerable<GroupShortDto>>> GetLookupAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<GroupShortDto>>> GetActiveLookupAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<GroupShortDto>>> GetLookupByCourseIdAsync(int courseId, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupShortDto>>> GetLookupByMentorIdAsync(int mentorId, CancellationToken ct = default);
}