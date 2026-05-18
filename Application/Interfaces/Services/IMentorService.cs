using Application.Dtos.MentorDto;
using Application.Results;
namespace Application.Interfaces.Services;

public interface IMentorService
{
    // ───── Basic CRUD ─────
    Task<Result<MentorResponseDto>> CreateAsync(MentorCreateDto dto, CancellationToken ct = default);
    Task<Result<MentorResponseDto>> UpdateAsync(int id, MentorUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<MentorResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);

    // ───── Queries ─────
    Task<Result<IEnumerable<MentorResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<MentorResponseDto>>> GetActiveAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<MentorResponseDto>>> GetBySpecializationAsync(string specialization, CancellationToken ct = default);

    // ───── Detailed ─────
    Task<Result<MentorWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default);
    Task<Result<MentorWithGroupsDto>> GetWithActiveGroupsAsync(int id, CancellationToken ct = default);
    Task<Result<MentorFullProfileDto>> GetFullProfileAsync(int id, CancellationToken ct = default);

    // ───── Admin Actions ─────
    Task<Result<MentorResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default);
    Task<Result<bool>> HasActiveGroupsAsync(int id, CancellationToken ct = default);
    Task<Result<int>> GetActiveGroupCountAsync(int id, CancellationToken ct = default);

    // ───── Invite System ─────
    Task<Result<bool>> SendInviteAsync(int id, CancellationToken ct = default);
    Task<Result<bool>> ResendInviteAsync(int id, CancellationToken ct = default);
    Task<Result<bool>> RevokeInviteAsync(int id, CancellationToken ct = default);

    // ───── Mentor Profile ─────
    Task<Result<MentorResponseDto>> GetMyProfileAsync(int userId, CancellationToken ct = default);
    Task<Result<MentorFullProfileDto>> GetMyFullProfileAsync(int userId, CancellationToken ct = default);
    Task<Result<MentorResponseDto>> UpdateMyProfileAsync(int userId, MentorUpdateDto dto, CancellationToken ct = default);

    // ───── Lookup (dropdowns/modals) ─────
    Task<Result<IEnumerable<MentorShortDto>>> GetLookupAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<MentorShortDto>>> GetActiveLookupAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<MentorShortDto>>> GetLookupBySpecializationAsync(string specialization, CancellationToken ct = default);
}