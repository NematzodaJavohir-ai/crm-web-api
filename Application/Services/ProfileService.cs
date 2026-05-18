using Application.Dtos.AuthDto;
using Application.Dtos.ProfileDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Enums;

namespace Application.Services;

public class ProfileService(IUnitOfWork uow) : IProfileService
{
    public async Task<Result<ProfileResponseDto>> GetMyProfileAsync(int userId, CancellationToken ct = default)
    {
        var profile = await uow.Profiles.GetByUserIdAsync(userId, ct);
        if (profile is null)
            return Result<ProfileResponseDto>.Fail("Profile not found", ErrorType.NotFound);

        return Result<ProfileResponseDto>.Ok(MapToResponseDto(profile));
    }

    public async Task<Result<ProfileResponseDto>> UpdateMyProfileAsync(int userId, UpdateProfileDto dto, CancellationToken ct = default)
    {
        var profile = await uow.Profiles.GetByUserIdAsync(userId, ct);
        if (profile is null)
            return Result<ProfileResponseDto>.Fail("Profile not found", ErrorType.NotFound);

        var user = await uow.Users.GetUserByIdAsync(userId, ct);
        if (user is null)
            return Result<ProfileResponseDto>.Fail("User not found", ErrorType.NotFound);

        if (dto.FirstName is not null) { profile.FirstName = dto.FirstName; user.FirstName = dto.FirstName; }
        if (dto.LastName is not null) { profile.LastName = dto.LastName; user.LastName = dto.LastName; }
        if (dto.Phone is not null) { profile.Phone = dto.Phone; user.PhoneNumber = dto.Phone; }
        if (dto.DateOfBirth.HasValue) profile.DateOfBirth = dto.DateOfBirth;
        if (dto.Address is not null) profile.Address = dto.Address;
        if (dto.TelegramUsername is not null) profile.TelegramUsername = dto.TelegramUsername;
        if (dto.LinkedInUrl is not null) profile.LinkedInUrl = dto.LinkedInUrl;
        if (dto.GithubUrl is not null) profile.GithubUrl = dto.GithubUrl;
        if (dto.AboutMe is not null) profile.AboutMe = dto.AboutMe;

        profile.UpdatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        uow.Profiles.Update(profile);
        await uow.Users.UpdateUserAsync(user);
        await uow.SaveChangesAsync(ct);

        return Result<ProfileResponseDto>.Ok(MapToResponseDto(profile));
    }

    public async Task<Result<bool>> UploadAvatarAsync(int userId, string photoUrl, CancellationToken ct = default)
    {
        var profile = await uow.Profiles.GetByUserIdAsync(userId, ct);
        if (profile is null)
            return Result<bool>.Fail("Profile not found", ErrorType.NotFound);

        profile.AvatarUrl = photoUrl;
        profile.UpdatedAt = DateTime.UtcNow;

        var user = await uow.Users.GetUserByIdAsync(userId, ct);
        if (user is not null)
        {
            user.PhotoUrl = photoUrl;
            user.UpdatedAt = DateTime.UtcNow;
            await uow.Users.UpdateUserAsync(user);
        }

        uow.Profiles.Update(profile);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> RemoveAvatarAsync(int userId, CancellationToken ct = default)
    {
        var profile = await uow.Profiles.GetByUserIdAsync(userId, ct);
        if (profile is null)
            return Result<bool>.Fail("Profile not found", ErrorType.NotFound);

        profile.AvatarUrl = null;
        profile.UpdatedAt = DateTime.UtcNow;

        var user = await uow.Users.GetUserByIdAsync(userId, ct);
        if (user is not null)
        {
            user.PhotoUrl = null;
            user.UpdatedAt = DateTime.UtcNow;
            await uow.Users.UpdateUserAsync(user);
        }

        uow.Profiles.Update(profile);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken ct = default)
    {
        var user = await uow.Users.GetUserByIdAsync(userId, ct);
        if (user is null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            return Result<bool>.Fail("Current password is incorrect", ErrorType.Validation);

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await uow.Users.UpdateUserAsync(user);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<ProfileResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var profile = await uow.Profiles.GetByIdAsync(id, ct);
        if (profile is null)
            return Result<ProfileResponseDto>.Fail("Profile not found", ErrorType.NotFound);

        return Result<ProfileResponseDto>.Ok(MapToResponseDto(profile));
    }

    public async Task<Result<ProfileResponseDto>> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var profile = await uow.Profiles.GetByUserIdAsync(userId, ct);
        if (profile is null)
            return Result<ProfileResponseDto>.Fail("Profile not found", ErrorType.NotFound);

        return Result<ProfileResponseDto>.Ok(MapToResponseDto(profile));
    }

    public async Task<Result<IEnumerable<ProfileResponseDto>>> SearchByNameAsync(string searchTerm, CancellationToken ct = default)
    {
        var profiles = await uow.Profiles.SearchByNameAsync(searchTerm, ct);
        return Result<IEnumerable<ProfileResponseDto>>.Ok(profiles.Select(MapToResponseDto));
    }

    public async Task<Result<bool>> PhoneExistsAsync(string phone, CancellationToken ct = default)
    {
        var exists = await uow.Profiles.PhoneExistsAsync(phone, ct);
        return Result<bool>.Ok(exists);
    }

    private static ProfileResponseDto MapToResponseDto(Domain.Entities.Profile p) => new()
    {
        Id = p.Id,
        UserId = p.UserId,
        FirstName = p.FirstName,
        LastName = p.LastName,
        AvatarUrl = p.AvatarUrl,
        Phone = p.Phone,
        Email = p.User?.Email,
        DateOfBirth = p.DateOfBirth,
        Address = p.Address,
        TelegramUsername = p.TelegramUsername,
        LinkedInUrl = p.LinkedInUrl,
        GithubUrl = p.GithubUrl,
        AboutMe = p.AboutMe,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}