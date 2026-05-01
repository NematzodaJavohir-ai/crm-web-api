using System;
using Application.Dtos.MentorDto;
using Application.Results;
namespace Application.Interfaces.Services;

public interface IMentorService
{
    // ───── Admin ─────
    Task<Result<MentorResponseDto>> CreateAsync(MentorCreateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<IEnumerable<MentorResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<MentorResponseDto>>> GetAllActiveAsync(CancellationToken ct = default);
    Task<Result<MentorResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default);

    // ───── Admin + Mentor ─────
    Task<Result<MentorResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<MentorWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default);

    // ───── Mentor (свой профиль) ─────
    Task<Result<MentorResponseDto>> GetMyProfileAsync(int userId, CancellationToken ct = default);
    Task<Result<MentorResponseDto>> UpdateMyProfileAsync(int userId, MentorUpdateDto dto, CancellationToken ct = default);
}