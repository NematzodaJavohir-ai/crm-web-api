using System;
using Application.Dtos.GroupStudentDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IGroupStudentService
{
    // ───── Admin + Mentor ─────
    Task<Result<GroupStudentResponseDto>> AddStudentAsync(AddStudentToGroupDto dto, CancellationToken ct = default);
    Task<Result<bool>> RemoveStudentAsync(RemoveStudentFromGroupDto dto, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupStudentResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupStudentResponseDto>>> GetActiveByGroupIdAsync(int groupId, CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<IEnumerable<GroupStudentResponseDto>>> GetMyGroupsAsync(int userId, CancellationToken ct = default);
}