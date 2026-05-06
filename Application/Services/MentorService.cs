using Application.Dtos.GroupDto;
using Application.Dtos.MentorDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class MentorService(IUnitOfWork uow) : IMentorService
{
    public async Task<Result<MentorResponseDto>> CreateAsync(MentorCreateDto dto, CancellationToken ct = default)
    {
        var emailExists = await uow.Users.EmailExistsAsync(dto.Email, ct);
        if (emailExists)
            return Result<MentorResponseDto>.Fail("Email already exists", ErrorType.Conflict);

        var phoneExists = await uow.Users.PhoneExistsAsync(dto.Phone, ct);
        if (phoneExists)
            return Result<MentorResponseDto>.Fail("PhoneNumber already exists", ErrorType.Conflict);

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.Phone ?? string.Empty,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Users.AddUserAsync(user, ct);
        await uow.SaveChangesAsync(ct);

        var mentor = new Mentor
        {
            UserId = user.Id,
            Phone = dto.Phone,
            Specialization = dto.Specialization,
            ExperienceYears = dto.ExperienceYears,
            Bio = dto.Bio,
            LinkedInUrl = dto.LinkedInUrl,
            GithubUrl = dto.GithubUrl,
            HireDate = DateTime.UtcNow,
            IsActive = true
        };

        await uow.Mentors.AddAsync(mentor, ct);
        await uow.SaveChangesAsync(ct);

        return Result<MentorResponseDto>.Ok(MapToResponseDto(mentor, user));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetWithUserAsync(id, ct);
        if (mentor is null)
            return Result<bool>.Fail("Mentor not found", ErrorType.NotFound);

        uow.Mentors.Delete(mentor);
        await uow.Users.DeleteUserAsync(mentor.UserId);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<IEnumerable<MentorResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var mentors = await uow.Mentors.GetAllAsync(ct);
        var result = mentors.Select(m => MapToResponseDto(m, m.User));

        return Result<IEnumerable<MentorResponseDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<MentorResponseDto>>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var mentors = await uow.Mentors.GetAllActiveAsync(ct);
        var result = mentors.Select(m => MapToResponseDto(m, m.User));

        return Result<IEnumerable<MentorResponseDto>>.Ok(result);
    }

    public async Task<Result<MentorResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetWithUserAsync(id, ct);
        if (mentor is null)
            return Result<MentorResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

        return Result<MentorResponseDto>.Ok(MapToResponseDto(mentor, mentor.User));
    }

    public async Task<Result<MentorWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetFullProfileAsync(id, ct);
        if (mentor is null)
            return Result<MentorWithGroupsDto>.Fail("Mentor not found", ErrorType.NotFound);

        var dto = new MentorWithGroupsDto
        {
            Id = mentor.Id,
            UserId = mentor.UserId,
            FullName = $"{mentor.User.FirstName} {mentor.User.LastName}",
            Email = mentor.User.Email,
            Phone = mentor.Phone,
            Specialization = mentor.Specialization,
            ExperienceYears = mentor.ExperienceYears,
            Bio = mentor.Bio,
            AvatarUrl = mentor.User.PhotoUrl,
            IsActive = mentor.IsActive,
            HireDate = mentor.HireDate,
            Groups = mentor.Groups.Select(g => new GroupShortDto
            {
                Id = g.Id,
                Name = g.Name,
                CourseName = g.Course.Name,
                MentorName = $"{mentor.User.FirstName} {mentor.User.LastName}",
                Status = g.Status
            })
        };

        return Result<MentorWithGroupsDto>.Ok(dto);
    }

    public async Task<Result<MentorResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetWithUserAsync(id, ct);
        if (mentor is null)
            return Result<MentorResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

        mentor.IsActive = isActive;
        mentor.User.IsActive = isActive;

        uow.Mentors.Update(mentor);
        await uow.Users.UpdateUserAsync(mentor.User);
        await uow.SaveChangesAsync(ct);

        return Result<MentorResponseDto>.Ok(MapToResponseDto(mentor, mentor.User));
    }

  public async Task<Result<MentorResponseDto>> UpdateMyProfileAsync(int userId, MentorUpdateDto dto, CancellationToken ct = default)
{
    var mentor = await uow.Mentors.GetByUserIdAsync(userId, ct);
    if (mentor is null)
        return Result<MentorResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

    var user = await uow.Users.GetUserByIdAsync(mentor.UserId, ct);
    if (user is null)
        return Result<MentorResponseDto>.Fail("User not found", ErrorType.NotFound);

    if (!string.IsNullOrEmpty(dto.FullName))
    {
        var nameParts = dto.FullName.Trim().Split(' ', 2);
        user.FirstName = nameParts[0];
        user.LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
    }


    if (!string.IsNullOrEmpty(dto.Email))
    {
        var emailExists = await uow.Users.EmailExistsAsync(dto.Email, ct);
        if (emailExists && user.Email != dto.Email)
            return Result<MentorResponseDto>.Fail("Email already exists", ErrorType.Conflict);
        user.Email = dto.Email;
    }

    if (dto.Phone is not null)
    {
        var phoneExists = await uow.Users.PhoneExistsAsync(dto.Phone, ct);
        if (phoneExists && user.PhoneNumber != dto.Phone)
            return Result<MentorResponseDto>.Fail("PhoneNumber already exists", ErrorType.Conflict);
        mentor.Phone = dto.Phone;
        user.PhoneNumber = dto.Phone;
    }

    if (dto.Specialization is not null) mentor.Specialization = dto.Specialization;
    if (dto.ExperienceYears.HasValue) mentor.ExperienceYears = dto.ExperienceYears;
    if (dto.Bio is not null) mentor.Bio = dto.Bio;
    if (dto.LinkedInUrl is not null) mentor.LinkedInUrl = dto.LinkedInUrl;
    if (dto.GithubUrl is not null) mentor.GithubUrl = dto.GithubUrl;

    user.UpdatedAt = DateTime.UtcNow;

    uow.Mentors.Update(mentor);
    await uow.Users.UpdateUserAsync(user);
    await uow.SaveChangesAsync(ct);

    return Result<MentorResponseDto>.Ok(MapToResponseDto(mentor, user));
}   
    public async Task<Result<MentorResponseDto>> GetMyProfileAsync(int userId, CancellationToken ct = default)
    {
        var mentor = await uow.Mentors.GetByUserIdAsync(userId, ct);
        if (mentor is null)
            return Result<MentorResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

        var user = await uow.Users.GetUserByIdAsync(mentor.UserId, ct);
        if (user is null)
            return Result<MentorResponseDto>.Fail("User not found", ErrorType.NotFound);

        return Result<MentorResponseDto>.Ok(MapToResponseDto(mentor, user));
    }

    private static MentorResponseDto MapToResponseDto(Mentor mentor, User user) => new()
    {
        Id = mentor.Id,
        UserId = user.Id,
        FullName = $"{user.FirstName} {user.LastName}",
        Email = user.Email,
        Phone = mentor.Phone,
        Specialization = mentor.Specialization,
        ExperienceYears = mentor.ExperienceYears,
        Bio = mentor.Bio,
        AvatarUrl = user.PhotoUrl,
        LinkedInUrl = mentor.LinkedInUrl,
        GithubUrl = mentor.GithubUrl,
        IsActive = mentor.IsActive,
        HireDate = mentor.HireDate
    };

   public async Task<Result<MentorResponseDto>> UpdateAsync(int id, MentorUpdateDto dto, CancellationToken ct)
{
    var mentor = await uow.Mentors.GetWithUserAsync(id, ct);
    if (mentor is null)
        return Result<MentorResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

    var user = mentor.User;
    if (user is null)
        return Result<MentorResponseDto>.Fail("User not found", ErrorType.NotFound);

    // --- Phone ---
    if (dto.Phone is not null && dto.Phone != mentor.Phone)
    {
        var phoneExists = await uow.Users.PhoneExistsAsync(dto.Phone, ct);
        if (phoneExists)
            return Result<MentorResponseDto>.Fail("PhoneNumber already exists", ErrorType.Conflict);

        mentor.Phone = dto.Phone;
        user.PhoneNumber = dto.Phone;
    }

    // --- Partial update ---
    if (dto.Specialization is not null)
        mentor.Specialization = dto.Specialization;

    if (dto.ExperienceYears.HasValue)
        mentor.ExperienceYears = dto.ExperienceYears.Value;

    if (dto.Bio is not null)
        mentor.Bio = dto.Bio;

    if (dto.LinkedInUrl is not null)
        mentor.LinkedInUrl = dto.LinkedInUrl;

    if (dto.GithubUrl is not null)
        mentor.GithubUrl = dto.GithubUrl;

    user.UpdatedAt = DateTime.UtcNow;

    uow.Mentors.Update(mentor);
    await uow.Users.UpdateUserAsync(user);
    await uow.SaveChangesAsync(ct);

    return Result<MentorResponseDto>.Ok(MapToResponseDto(mentor, user));
}
}