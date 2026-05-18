using Application.Dtos.GroupDto;
using Application.Dtos.PaymentDto;
using Application.Dtos.StudentDto;
using Application.Dtos.WeeklyResultDto;
using Application.Interfaces.Service;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class StudentService(IUnitOfWork uow, ICacheService cacheService, ILogger<StudentService> logger) : IStudentService
{
    public async Task<Result<StudentResponseDto>> CreateAsync(StudentCreateDto dto, CancellationToken ct = default)
    {
        logger.LogInformation("Creating new student with email: {Email}", dto.Email);

        var emailExists = await uow.Users.EmailExistsAsync(dto.Email, ct);
        if (emailExists)
            return Result<StudentResponseDto>.Fail("Email already exists", ErrorType.Conflict);

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.Phone ?? string.Empty,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId = (int)UserRole.Student,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Users.AddUserAsync(user, ct);
        await uow.SaveChangesAsync(ct);

        var student = new Student
        {
            UserId = user.Id,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            Balance = 0,
            IsActive = true,
            EnrollDate = DateTime.UtcNow
        };

        await uow.Students.AddAsync(student, ct);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Student created with ID: {StudentId}", student.Id);

        return Result<StudentResponseDto>.Ok(MapToResponseDto(student, user));
    }

    public async Task<Result<StudentResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByIdAsync(id, ct);
        if (student is null)
            return Result<StudentResponseDto>.Fail("Student not found", ErrorType.NotFound);

        return Result<StudentResponseDto>.Ok(MapToResponseDto(student, student.User));
    }

    public async Task<Result<StudentResponseDto>> UpdateAsync(int id, StudentUpdateDto dto, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByIdAsync(id, ct);
        if (student is null)
            return Result<StudentResponseDto>.Fail("Student not found", ErrorType.NotFound);

        var user = await uow.Users.GetUserByIdAsync(student.UserId, ct);
        if (user is null)
            return Result<StudentResponseDto>.Fail("User not found", ErrorType.NotFound);

        if (dto.FirstName is not null) user.FirstName = dto.FirstName;
        if (dto.LastName is not null) user.LastName = dto.LastName;
        if (dto.Phone is not null) { student.Phone = dto.Phone; user.PhoneNumber = dto.Phone; }
        if (dto.DateOfBirth.HasValue) student.DateOfBirth = dto.DateOfBirth;
        if (dto.PhotoUrl is not null) user.PhotoUrl = dto.PhotoUrl;
        if (dto.TelegramUsername is not null) student.TelegramUsername = dto.TelegramUsername;
        if (dto.GithubUrl is not null) student.GithubUrl = dto.GithubUrl;
        if (dto.AboutMe is not null) student.AboutMe = dto.AboutMe;

        user.UpdatedAt = DateTime.UtcNow;

        uow.Students.Update(student);
        await uow.Users.UpdateUserAsync(user);
        await uow.SaveChangesAsync(ct);

        return Result<StudentResponseDto>.Ok(MapToResponseDto(student, user));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByIdAsync(id, ct);
        if (student is null)
            return Result<bool>.Fail("Student not found", ErrorType.NotFound);

        uow.Students.Delete(student);
        await uow.Users.DeleteUserAsync(student.UserId);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<IEnumerable<StudentResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var students = await uow.Students.GetAllAsync(ct);
        return Result<IEnumerable<StudentResponseDto>>.Ok(students.Select(s => MapToResponseDto(s, s.User)));
    }

    public async Task<Result<IEnumerable<StudentResponseDto>>> GetActiveAsync(CancellationToken ct = default)
    {
        var students = await uow.Students.GetAllAsync(ct);
        return Result<IEnumerable<StudentResponseDto>>.Ok(students.Where(s => s.IsActive).Select(s => MapToResponseDto(s, s.User)));
    }

    public async Task<Result<StudentResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByIdAsync(id, ct);
        if (student is null)
            return Result<StudentResponseDto>.Fail("Student not found", ErrorType.NotFound);

        student.IsActive = isActive;
        student.User.IsActive = isActive;

        uow.Students.Update(student);
        await uow.SaveChangesAsync(ct);

        return Result<StudentResponseDto>.Ok(MapToResponseDto(student, student.User));
    }

    public async Task<Result<IEnumerable<StudentResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var students = await uow.Students.GetByGroupIdAsync(groupId, ct);
        return Result<IEnumerable<StudentResponseDto>>.Ok(students.Select(s => MapToResponseDto(s, s.User)));
    }

    public async Task<Result<StudentWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default)
    {
        var student = await uow.Students.GetWithGroupsAsync(id, ct);
        if (student is null)
            return Result<StudentWithGroupsDto>.Fail("Student not found", ErrorType.NotFound);

        return Result<StudentWithGroupsDto>.Ok(new StudentWithGroupsDto
        {
            Id = student.Id,
            UserId = student.UserId,
            FullName = $"{student.User.FirstName} {student.User.LastName}",
            Email = student.User.Email,
            Phone = student.Phone,
            PhotoUrl = student.User.PhotoUrl,
            TelegramUsername = student.TelegramUsername,
            GithubUrl = student.GithubUrl,
            AboutMe = student.AboutMe,
            DateOfBirth = student.DateOfBirth,
            Balance = student.Balance,
            IsActive = student.IsActive,
            EnrollDate = student.EnrollDate,
            Groups = student.GroupStudents
                .Where(gs => gs.IsActive)
                .Select(gs => new GroupShortDto
                {
                    Id = gs.Group.Id,
                    Name = gs.Group.Name,
                    CourseName = gs.Group.Course?.Name ?? "",
                    MentorName = gs.Group.Mentor?.User != null
                        ? $"{gs.Group.Mentor.User.FirstName} {gs.Group.Mentor.User.LastName}"
                        : "",
                    Status = gs.Group.Status.ToString(),
                    StudentCount = gs.Group.GroupStudents.Count(x => x.IsActive),
                    StartDate = gs.Group.StartDate
                })
        });
    }

    public async Task<Result<StudentWithPaymentsDto>> GetWithPaymentsAsync(int id, CancellationToken ct = default)
    {
        var student = await uow.Students.GetWithPaymentsAsync(id, ct);
        if (student is null)
            return Result<StudentWithPaymentsDto>.Fail("Student not found", ErrorType.NotFound);

        return Result<StudentWithPaymentsDto>.Ok(new StudentWithPaymentsDto
        {
            Id = student.Id,
            UserId = student.UserId,
            FullName = $"{student.User.FirstName} {student.User.LastName}",
            Email = student.User.Email,
            Phone = student.Phone,
            PhotoUrl = student.User.PhotoUrl,
            Balance = student.Balance,
            Payments = student.Payments.Select(p => new PaymentShortDto
            {
                Id = p.Id,
                Amount = p.Amount,
                Type = p.Type.ToString(),
                Method = p.Method.ToString(),
                Date = p.Date,
                IsConfirmed = p.IsConfirmed,
                GroupName = p.Group?.Name
            })
        });
    }

    public async Task<Result<StudentFullProfileDto>> GetFullProfileAsync(int id, CancellationToken ct = default)
    {
        var student = await uow.Students.GetWithGroupsAsync(id, ct);
        if (student is null)
            return Result<StudentFullProfileDto>.Fail("Student not found", ErrorType.NotFound);

        return Result<StudentFullProfileDto>.Ok(new StudentFullProfileDto
        {
            Id = student.Id,
            UserId = student.UserId,
            FullName = $"{student.User.FirstName} {student.User.LastName}",
            Email = student.User.Email,
            Phone = student.Phone,
            PhotoUrl = student.User.PhotoUrl,
            TelegramUsername = student.TelegramUsername,
            GithubUrl = student.GithubUrl,
            AboutMe = student.AboutMe,
            DateOfBirth = student.DateOfBirth,
            Balance = student.Balance,
            IsActive = student.IsActive,
            EnrollDate = student.EnrollDate,
            TotalGroups = student.GroupStudents.Count,
            ActiveGroups = student.GroupStudents.Count(gs => gs.IsActive),
            Groups = student.GroupStudents.Select(gs => new GroupShortDto
            {
                Id = gs.Group.Id,
                Name = gs.Group.Name,
                CourseName = gs.Group.Course?.Name ?? "",
                MentorName = gs.Group.Mentor?.User != null
                    ? $"{gs.Group.Mentor.User.FirstName} {gs.Group.Mentor.User.LastName}"
                    : "",
                Status = gs.Group.Status.ToString(),
                StudentCount = gs.Group.GroupStudents.Count(x => x.IsActive),
                StartDate = gs.Group.StartDate
            })
        });
    }

    public async Task<Result<decimal>> GetBalanceAsync(int id, CancellationToken ct = default)
    {
        var balance = await uow.Students.GetBalanceAsync(id, ct);
        return Result<decimal>.Ok(balance);
    }

    public async Task<Result<bool>> AddToBalanceAsync(int id, decimal amount, CancellationToken ct = default)
    {
        await uow.Students.UpdateBalanceAsync(id, amount, ct);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> DeductFromBalanceAsync(int id, decimal amount, CancellationToken ct = default)
    {
        await uow.Students.UpdateBalanceAsync(id, -amount, ct);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<IEnumerable<StudentResponseDto>>> GetDebtorsAsync(CancellationToken ct = default)
    {
        var students = await uow.Students.GetDebtorsAsync(ct);
        return Result<IEnumerable<StudentResponseDto>>.Ok(students.Select(s => MapToResponseDto(s, s.User)));
    }

    public async Task<Result<StudentResponseDto>> GetMyProfileAsync(int userId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<StudentResponseDto>.Fail("Student not found", ErrorType.NotFound);

        return Result<StudentResponseDto>.Ok(MapToResponseDto(student, student.User));
    }

    public async Task<Result<StudentFullProfileDto>> GetMyFullProfileAsync(int userId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<StudentFullProfileDto>.Fail("Student not found", ErrorType.NotFound);

        return await GetFullProfileAsync(student.Id, ct);
    }

    public async Task<Result<StudentResponseDto>> UpdateMyProfileAsync(int userId, StudentUpdateDto dto, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<StudentResponseDto>.Fail("Student not found", ErrorType.NotFound);

        return await UpdateAsync(student.Id, dto, ct);
    }

    public async Task<Result<IEnumerable<StudentShortDto>>> GetLookupAsync(CancellationToken ct = default)
    {
        var students = await uow.Students.GetAllAsync(ct);
        return Result<IEnumerable<StudentShortDto>>.Ok(students.Select(s => new StudentShortDto
        {
            Id = s.Id,
            FullName = $"{s.User.FirstName} {s.User.LastName}",
            Email = s.User.Email,
            PhotoUrl = s.User.PhotoUrl,
            ActiveGroupsCount = s.GroupStudents.Count(gs => gs.IsActive)
        }));
    }

    public async Task<Result<IEnumerable<StudentShortDto>>> GetLookupByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var students = await uow.Students.GetByGroupIdAsync(groupId, ct);
        return Result<IEnumerable<StudentShortDto>>.Ok(students.Select(s => new StudentShortDto
        {
            Id = s.Id,
            FullName = $"{s.User.FirstName} {s.User.LastName}",
            Email = s.User.Email,
            PhotoUrl = s.User.PhotoUrl,
            ActiveGroupsCount = s.GroupStudents.Count(gs => gs.IsActive)
        }));
    }

    private static StudentResponseDto MapToResponseDto(Student s, User u) => new()
    {
        Id = s.Id,
        UserId = u.Id,
        FullName = $"{u.FirstName} {u.LastName}",
        Email = u.Email,
        Phone = s.Phone,
        PhotoUrl = u.PhotoUrl,
        TelegramUsername = s.TelegramUsername,
        GithubUrl = s.GithubUrl,
        AboutMe = s.AboutMe,
        DateOfBirth = s.DateOfBirth,
        Balance = s.Balance,
        IsActive = s.IsActive,
        EnrollDate = s.EnrollDate,
        ActiveGroupsCount = s.GroupStudents?.Count(gs => gs.IsActive) ?? 0
    };
}