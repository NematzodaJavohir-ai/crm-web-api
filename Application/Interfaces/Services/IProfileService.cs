using Application.Dtos.AuthDto;
using Application.Dtos.ProfileDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IProfileService
{
    // ───── Own Profile ─────
    Task<Result<ProfileResponseDto>> GetMyProfileAsync(int userId, CancellationToken ct = default);
    Task<Result<ProfileResponseDto>> UpdateMyProfileAsync(int userId, UpdateProfileDto dto, CancellationToken ct = default);
    Task<Result<bool>> UploadAvatarAsync(int userId, string photoUrl, CancellationToken ct = default);
    Task<Result<bool>> RemoveAvatarAsync(int userId, CancellationToken ct = default);
    Task<Result<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken ct = default);

    // ───── Admin ─────
    Task<Result<ProfileResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<ProfileResponseDto>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<Result<IEnumerable<ProfileResponseDto>>> SearchByNameAsync(string searchTerm, CancellationToken ct = default);
    Task<Result<bool>> PhoneExistsAsync(string phone, CancellationToken ct = default);
}