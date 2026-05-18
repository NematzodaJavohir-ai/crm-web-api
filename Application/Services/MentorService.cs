using Application.Dtos.GroupDto;
using Application.Dtos.MentorDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class MentorService(IUnitOfWork uow, IEmailService emailService) : IMentorService
{
    public async Task<Result<MentorResponseDto>> CreateAsync(MentorCreateDto dto, CancellationToken ct = default)
    {
        var emailExists = await uow.Users.EmailExistsAsync(dto.Email, ct);
        if (emailExists)
            return Result<MentorResponseDto>.Fail("Email already exists", ErrorType.Conflict);

        if (!string.IsNullOrEmpty(dto.Phone))
        {
            var phoneExists = await uow.Users.PhoneExistsAsync(dto.Phone, ct);
            if (phoneExists)
                return Result<MentorResponseDto>.Fail("Phone number already exists", ErrorType.Conflict);
        }

        var inviteToken = GenerateInviteToken();

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.Phone ?? string.Empty,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
            RoleId = (int)UserRole.Mentor,
            IsActive = false,
            IsPasswordSet = false,
            InviteToken = inviteToken,
            InviteTokenExpiry = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await uow.Users.AddUserAsync(user, ct);
        await uow.SaveChangesAsync(ct);

        var mentor = new Mentor
        {
            UserId = user.Id,
            Phone = dto.Phone,
            Specialization = dto.Specialization,
            HireDate = DateTime.UtcNow,
            IsActive = false
        };

        await uow.Mentors.AddAsync(mentor, ct);
        await uow.SaveChangesAsync(ct);

        await SendInviteEmailAsync(user.Email, user.FirstName, inviteToken);

        return Result<MentorResponseDto>.Ok(MapToResponseDto(mentor, user));
    }

    public async Task<Result<MentorResponseDto>> UpdateAsync(int id, MentorUpdateDto dto, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetByIdAsync(id, ct);
        if (mentor is null)
            return Result<MentorResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

        var user = await uow.Users.GetUserByIdAsync(mentor.UserId, ct);
        if (user is null)
            return Result<MentorResponseDto>.Fail("User not found", ErrorType.NotFound);

        if (!string.IsNullOrEmpty(dto.Phone) && dto.Phone != user.PhoneNumber)
        {
            var phoneExists = await uow.Users.PhoneExistsAsync(dto.Phone, ct);
            if (phoneExists)
                return Result<MentorResponseDto>.Fail("Phone number already exists", ErrorType.Conflict);

            mentor.Phone = dto.Phone;
            user.PhoneNumber = dto.Phone;
        }

        if (dto.FirstName is not null) user.FirstName = dto.FirstName;
        if (dto.LastName is not null) user.LastName = dto.LastName;
        if (dto.Specialization is not null) mentor.Specialization = dto.Specialization;
        if (dto.ExperienceYears.HasValue) mentor.ExperienceYears = dto.ExperienceYears.Value;
        if (dto.Bio is not null) mentor.Bio = dto.Bio;
        if (dto.LinkedInUrl is not null) mentor.LinkedInUrl = dto.LinkedInUrl;
        if (dto.GithubUrl is not null) mentor.GithubUrl = dto.GithubUrl;
        if (dto.PhotoUrl is not null) user.PhotoUrl = dto.PhotoUrl;

        user.UpdatedAt = DateTime.UtcNow;

        uow.Mentors.Update(mentor);
        await uow.Users.UpdateUserAsync(user);
        await uow.SaveChangesAsync(ct);

        return Result<MentorResponseDto>.Ok(MapToResponseDto(mentor, user));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetByIdAsync(id, ct);
        if (mentor is null)
            return Result<bool>.Fail("Mentor not found", ErrorType.NotFound);

        var activeGroups = await uow.Mentors.GetActiveGroupCountAsync(id, ct);
        if (activeGroups > 0)
            return Result<bool>.Fail($"Cannot delete mentor with {activeGroups} active groups", ErrorType.Conflict);

        uow.Mentors.Delete(mentor);
        await uow.Users.DeleteUserAsync(mentor.UserId);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<MentorResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetByIdAsync(id, ct);
        if (mentor is null)
            return Result<MentorResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

        return Result<MentorResponseDto>.Ok(MapToResponseDto(mentor, mentor.User));
    }

    public async Task<Result<IEnumerable<MentorResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var mentors = await uow.Mentors.GetAllAsync(ct);
        var result = mentors.Select(m => MapToResponseDto(m, m.User));
        return Result<IEnumerable<MentorResponseDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<MentorResponseDto>>> GetActiveAsync(CancellationToken ct = default)
    {
        var mentors = await uow.Mentors.GetActiveAsync(ct);
        var result = mentors.Select(m => MapToResponseDto(m, m.User));
        return Result<IEnumerable<MentorResponseDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<MentorResponseDto>>> GetBySpecializationAsync(string specialization, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(specialization))
            return Result<IEnumerable<MentorResponseDto>>.Fail("Specialization is required", ErrorType.Validation);

        var mentors = await uow.Mentors.GetBySpecializationAsync(specialization, ct);
        var result = mentors.Select(m => MapToResponseDto(m, m.User));
        return Result<IEnumerable<MentorResponseDto>>.Ok(result);
    }

    public async Task<Result<MentorWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetWithGroupsAsync(id, ct);
        if (mentor is null)
            return Result<MentorWithGroupsDto>.Fail("Mentor not found", ErrorType.NotFound);

        return Result<MentorWithGroupsDto>.Ok(MapToWithGroupsDto(mentor));
    }

    public async Task<Result<MentorWithGroupsDto>> GetWithActiveGroupsAsync(int id, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetWithGroupsAsync(id, ct);
        if (mentor is null)
            return Result<MentorWithGroupsDto>.Fail("Mentor not found", ErrorType.NotFound);

        return Result<MentorWithGroupsDto>.Ok(MapToWithGroupsDto(mentor, onlyActive: true));
    }

    public async Task<Result<MentorFullProfileDto>> GetFullProfileAsync(int id, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetWithGroupsAsync(id, ct);
        if (mentor is null)
            return Result<MentorFullProfileDto>.Fail("Mentor not found", ErrorType.NotFound);

        return Result<MentorFullProfileDto>.Ok(MapToFullProfileDto(mentor));
    }

    public async Task<Result<MentorResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetByIdAsync(id, ct);
        if (mentor is null)
            return Result<MentorResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

        var user = await uow.Users.GetUserByIdAsync(mentor.UserId, ct);
        if (user is null)
            return Result<MentorResponseDto>.Fail("User not found", ErrorType.NotFound);

        if (mentor.IsActive == isActive)
            return Result<MentorResponseDto>.Fail($"Mentor is already {(isActive ? "active" : "inactive")}", ErrorType.Validation);

        if (!isActive)
        {
            var activeGroups = await uow.Mentors.GetActiveGroupCountAsync(id, ct);
            if (activeGroups > 0)
                return Result<MentorResponseDto>.Fail($"Cannot deactivate mentor with {activeGroups} active groups", ErrorType.Conflict);
        }

        mentor.IsActive = isActive;
        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;

        uow.Mentors.Update(mentor);
        await uow.Users.UpdateUserAsync(user);
        await uow.SaveChangesAsync(ct);

        return Result<MentorResponseDto>.Ok(MapToResponseDto(mentor, user));
    }

    public async Task<Result<bool>> HasActiveGroupsAsync(int id, CancellationToken ct = default)
    {
        var exists = await uow.Mentors.ExistsAsync(id, ct);
        if (!exists)
            return Result<bool>.Fail("Mentor not found", ErrorType.NotFound);

        var count = await uow.Mentors.GetActiveGroupCountAsync(id, ct);
        return Result<bool>.Ok(count > 0);
    }

    public async Task<Result<int>> GetActiveGroupCountAsync(int id, CancellationToken ct = default)
    {
        var exists = await uow.Mentors.ExistsAsync(id, ct);
        if (!exists)
            return Result<int>.Fail("Mentor not found", ErrorType.NotFound);

        var count = await uow.Mentors.GetActiveGroupCountAsync(id, ct);
        return Result<int>.Ok(count);
    }

    public async Task<Result<bool>> SendInviteAsync(int id, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetByIdAsync(id, ct);
        if (mentor is null)
            return Result<bool>.Fail("Mentor not found", ErrorType.NotFound);

        var user = await uow.Users.GetUserByIdAsync(mentor.UserId, ct);
        if (user is null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        if (user.IsPasswordSet)
            return Result<bool>.Fail("Mentor already accepted the invite", ErrorType.Conflict);

        var inviteToken = GenerateInviteToken();
        user.InviteToken = inviteToken;
        user.InviteTokenExpiry = DateTime.UtcNow.AddDays(7);
        user.UpdatedAt = DateTime.UtcNow;

        await uow.Users.UpdateUserAsync(user);
        await uow.SaveChangesAsync(ct);
        await SendInviteEmailAsync(user.Email, user.FirstName, inviteToken);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> ResendInviteAsync(int id, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetByIdAsync(id, ct);
        if (mentor is null)
            return Result<bool>.Fail("Mentor not found", ErrorType.NotFound);

        var user = await uow.Users.GetUserByIdAsync(mentor.UserId, ct);
        if (user is null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        if (user.IsPasswordSet)
            return Result<bool>.Fail("Mentor already accepted the invite", ErrorType.Conflict);

        var inviteToken = GenerateInviteToken();
        user.InviteToken = inviteToken;
        user.InviteTokenExpiry = DateTime.UtcNow.AddDays(7);
        user.UpdatedAt = DateTime.UtcNow;

        await uow.Users.UpdateUserAsync(user);
        await uow.SaveChangesAsync(ct);
        await SendInviteEmailAsync(user.Email, user.FirstName, inviteToken);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> RevokeInviteAsync(int id, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetByIdAsync(id, ct);
        if (mentor is null)
            return Result<bool>.Fail("Mentor not found", ErrorType.NotFound);

        var user = await uow.Users.GetUserByIdAsync(mentor.UserId, ct);
        if (user is null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        if (user.IsPasswordSet)
            return Result<bool>.Fail("Cannot revoke — mentor already accepted", ErrorType.Conflict);

        user.InviteToken = null;
        user.InviteTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;

        await uow.Users.UpdateUserAsync(user);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<MentorResponseDto>> GetMyProfileAsync(int userId, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetByUserIdAsync(userId, ct);
        if (mentor is null)
            return Result<MentorResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

        return Result<MentorResponseDto>.Ok(MapToResponseDto(mentor, mentor.User));
    }

    public async Task<Result<MentorFullProfileDto>> GetMyFullProfileAsync(int userId, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetByUserIdAsync(userId, ct);
        if (mentor is null)
            return Result<MentorFullProfileDto>.Fail("Mentor not found", ErrorType.NotFound);

        return await GetFullProfileAsync(mentor.Id, ct);
    }

    public async Task<Result<MentorResponseDto>> UpdateMyProfileAsync(int userId, MentorUpdateDto dto, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetByUserIdAsync(userId, ct);
        if (mentor is null)
            return Result<MentorResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

        return await UpdateAsync(mentor.Id, dto, ct);
    }

    public async Task<Result<IEnumerable<MentorShortDto>>> GetLookupAsync(CancellationToken ct = default)
    {
        var mentors = await uow.Mentors.GetAllAsync(ct);
        var result = mentors.Select(MapToShortDto);
        return Result<IEnumerable<MentorShortDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<MentorShortDto>>> GetActiveLookupAsync(CancellationToken ct = default)
    {
        var mentors = await uow.Mentors.GetActiveAsync(ct);
        var result = mentors.Select(MapToShortDto);
        return Result<IEnumerable<MentorShortDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<MentorShortDto>>> GetLookupBySpecializationAsync(string specialization, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(specialization))
            return Result<IEnumerable<MentorShortDto>>.Fail("Specialization is required", ErrorType.Validation);

        var mentors = await uow.Mentors.GetBySpecializationAsync(specialization, ct);
        var result = mentors.Select(MapToShortDto);
        return Result<IEnumerable<MentorShortDto>>.Ok(result);
    }

    // ───── Helpers ─────

    private static string GenerateInviteToken()
        => Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .Replace("=", "");

    private async Task SendInviteEmailAsync(string email, string firstName, string token)
    {
        var inviteLink = $"https://yourcrm.com/accept-invite?token={token}";

        await emailService.SendEmail(to: email, subject: "You are invited to CRM Platform!", body: $"""
            <html>
            <body style="font-family:Arial,sans-serif;padding:40px;background:#f5f6fa;">
                <div style="max-width:500px;margin:0 auto;background:white;border-radius:16px;padding:40px;">
                    <h2 style="color:#6C63FF;">You have been invited! 🎉</h2>
                    <p>Hello <strong>{firstName}</strong>,</p>
                    <p>You have been invited to join our CRM platform as a <strong>Mentor</strong>.</p>
                    <p>Click below to set your password:</p>
                    <a href="{inviteLink}" style="display:inline-block;background:#6C63FF;color:white;padding:14px 28px;border-radius:10px;text-decoration:none;font-weight:bold;">Accept Invitation</a>
                    <p style="color:#9A9BB0;font-size:13px;">Expires in 7 days.</p>
                </div>
            </body>
            </html>
        """);
    }

    private static MentorResponseDto MapToResponseDto(Mentor m, User u) => new()
    {
        Id = m.Id,
        UserId = u.Id,
        FullName = $"{u.FirstName} {u.LastName}",
        Email = u.Email,
        Phone = m.Phone,
        PhotoUrl = u.PhotoUrl,
        Specialization = m.Specialization,
        ExperienceYears = m.ExperienceYears,
        Bio = m.Bio,
        LinkedInUrl = m.LinkedInUrl,
        GithubUrl = m.GithubUrl,
        IsActive = m.IsActive,
        IsPasswordSet = u.IsPasswordSet,
        InviteStatus = u.InviteStatus,
        HireDate = m.HireDate,
        ActiveGroupCount = m.Groups.Count(g => g.Status == GroupStatus.Active)
    };

    private static MentorWithGroupsDto MapToWithGroupsDto(Mentor m, bool onlyActive = false)
    {
        var groups = onlyActive ? m.Groups.Where(g => g.Status == GroupStatus.Active) : m.Groups;

        return new MentorWithGroupsDto
        {
            Id = m.Id,
            UserId = m.UserId,
            FullName = $"{m.User.FirstName} {m.User.LastName}",
            Email = m.User.Email,
            Phone = m.Phone,
            PhotoUrl = m.User.PhotoUrl,
            Specialization = m.Specialization,
            ExperienceYears = m.ExperienceYears,
            Bio = m.Bio,
            LinkedInUrl = m.LinkedInUrl,
            GithubUrl = m.GithubUrl,
            IsActive = m.IsActive,
            IsPasswordSet = m.User.IsPasswordSet,
            InviteStatus = m.User.InviteStatus,
            HireDate = m.HireDate,
            ActiveGroupCount = m.Groups.Count(g => g.Status == GroupStatus.Active),
            Groups = groups.Select(g => new GroupShortDto
            {
                Id = g.Id,
                Name = g.Name,
                CourseName = g.Course?.Name ?? "",
                MentorName = $"{m.User.FirstName} {m.User.LastName}",
                Status = g.Status.ToString(),
                StudentCount = g.GroupStudents.Count(gs => gs.IsActive),
                StartDate = g.StartDate
            }).ToList()
        };
    }

    private static MentorFullProfileDto MapToFullProfileDto(Mentor m)
    {
        var activeGroups = m.Groups.Where(g => g.Status == GroupStatus.Active).ToList();
        var completedGroups = m.Groups.Where(g => g.Status == GroupStatus.Completed).ToList();
        var totalStudents = m.Groups.SelectMany(g => g.GroupStudents).Select(gs => gs.StudentId).Distinct().Count();
        var activeStudents = activeGroups.SelectMany(g => g.GroupStudents.Where(gs => gs.IsActive)).Select(gs => gs.StudentId).Distinct().Count();

        return new MentorFullProfileDto
        {
            Id = m.Id,
            UserId = m.UserId,
            FullName = $"{m.User.FirstName} {m.User.LastName}",
            Email = m.User.Email,
            Phone = m.Phone,
            Specialization = m.Specialization,
            ExperienceYears = m.ExperienceYears,
            Bio = m.Bio,
            AvatarUrl = m.User.PhotoUrl,
            LinkedInUrl = m.LinkedInUrl,
            GithubUrl = m.GithubUrl,
            IsActive = m.IsActive,
            IsPasswordSet = m.User.IsPasswordSet,
            InviteStatus = m.User.InviteStatus,
            HireDate = m.HireDate,
            CreatedAt = m.User.CreatedAt,
            TotalStudents = totalStudents,
            ActiveStudents = activeStudents,
            TotalGroups = m.Groups.Count,
            ActiveGroups = activeGroups.Count,
            CompletedGroups = completedGroups.Count,
            Groups = m.Groups.Select(g => new GroupShortDto
            {
                Id = g.Id,
                Name = g.Name,
                CourseName = g.Course?.Name ?? "",
                MentorName = $"{m.User.FirstName} {m.User.LastName}",
                Status = g.Status.ToString(),
                StudentCount = g.GroupStudents.Count(gs => gs.IsActive),
                StartDate = g.StartDate
            }).ToList()
        };
    }

    private static MentorShortDto MapToShortDto(Mentor m) => new()
    {
        Id = m.Id,
        FullName = $"{m.User.FirstName} {m.User.LastName}",
        Specialization = m.Specialization,
        PhotoUrl = m.User.PhotoUrl,
        ActiveGroupCount = m.Groups.Count(g => g.Status == GroupStatus.Active)
    };
}