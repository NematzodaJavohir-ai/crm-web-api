using Application.Dtos.GroupDto;
using Application.Dtos.StudentDto;
using Application.Dtos.WeeklyResultDto;
using Application.Interfaces.Service;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class StudentService(IUnitOfWork _uow,ICacheService _cacheService,ILogger<StudentService> _logger) : IStudentService
{
    public async Task<Result<StudentResponseDto>> CreateAsync(StudentCreateDto dto, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating new student with email: {Email}", dto.Email);

            var emailExists = await _uow.Users.EmailExistsAsync(dto.Email, ct);
            if (emailExists)
            {
                _logger.LogWarning("Email {Email} already exists", dto.Email);
                return Result<StudentResponseDto>.Fail("Email already exists", ErrorType.Conflict);
            }

            var nameParts = dto.FullName.Trim().Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = dto.Email,
                PhoneNumber = dto.Phone ?? string.Empty,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhotoUrl = dto.PhotoUrl,
                RoleId = (int)UserRole.Student,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Users.AddUserAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            var student = new Student
            {
                UserId = user.Id,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                TelegramUsername = dto.TelegramUsername,
                GithubUrl = dto.GithubUrl,
                AboutMe = dto.AboutMe,
                EnrollDate = DateTime.UtcNow
            };

            await _uow.Students.AddAsync(student, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully created student with ID: {StudentId}", student.Id);

            var response = MapToResponseDto(student, user);
            
            // Cache the newly created student
            var cacheKey = GetStudentCacheKey(student.Id);
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            
            return Result<StudentResponseDto>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student with email: {Email}", dto.Email);
            return Result<StudentResponseDto>.Fail("An error occurred while creating the student", ErrorType.Unknown);
        }
    }

    public async Task<Result<StudentResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid student ID: {Id}", id);
                return Result<StudentResponseDto>.Fail("Invalid student ID", ErrorType.Validation);
            }

            var cacheKey = GetStudentCacheKey(id);
            
            // Try to get from cache
            var cachedStudent = await _cacheService.GetAsync<StudentResponseDto>(cacheKey);
            if (cachedStudent != null)
            {
                _logger.LogDebug("Student {Id} retrieved from cache", id);
                return Result<StudentResponseDto>.Ok(cachedStudent);
            }

            _logger.LogDebug("Student {Id} not found in cache, retrieving from database", id);

            var student = await _uow.Students.GetWithUserAsync(id, ct);
            if (student is null)
            {
                _logger.LogWarning("Student with ID {Id} not found", id);
                return Result<StudentResponseDto>.Fail($"Student with ID {id} not found", ErrorType.NotFound);
            }

            if (student.User is null)
            {
                _logger.LogError("Student user data is corrupted for student ID: {Id}", id);
                return Result<StudentResponseDto>.Fail("Student user data is corrupted", ErrorType.NotFound);
            }

            var response = MapToResponseDto(student, student.User);
            
            // Cache the result
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            _logger.LogDebug("Student {Id} cached successfully", id);

            return Result<StudentResponseDto>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student by ID: {Id}", id);
            return Result<StudentResponseDto>.Fail("An error occurred while retrieving the student", ErrorType.Unknown);
        }
    }

    public async Task<Result<StudentResponseDto>> UpdateAsync(int id, StudentUpdateDto dto, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Updating student with ID: {Id}", id);

            var student = await _uow.Students.GetByIdAsync(id, ct);
            if (student is null)
            {
                _logger.LogWarning("Student with ID {Id} not found for update", id);
                return Result<StudentResponseDto>.Fail("Student not found", ErrorType.NotFound);
            }

            var user = await _uow.Users.GetUserByIdAsync(student.UserId, ct);
            if (user is null)
            {
                _logger.LogError("User not found for student ID: {Id}", id);
                return Result<StudentResponseDto>.Fail("User not found", ErrorType.NotFound);
            }

            // Update fields
            if (dto.Phone is not null)
            {
                student.Phone = dto.Phone;
                user.PhoneNumber = dto.Phone;
            }

            if (dto.DateOfBirth.HasValue) student.DateOfBirth = dto.DateOfBirth;
            if (dto.TelegramUsername is not null) student.TelegramUsername = dto.TelegramUsername;
            if (dto.GithubUrl is not null) student.GithubUrl = dto.GithubUrl;
            if (dto.AboutMe is not null) student.AboutMe = dto.AboutMe;

            user.UpdatedAt = DateTime.UtcNow;

            _uow.Students.Update(student);
            await _uow.Users.UpdateUserAsync(user);
            await _uow.SaveChangesAsync(ct);

            // Invalidate cache
            var cacheKey = GetStudentCacheKey(id);
            await _cacheService.RemoveAsync(cacheKey);
            await InvalidateStudentListCacheAsync();

            _logger.LogInformation("Successfully updated student with ID: {Id}", id);

            var response = MapToResponseDto(student, user);
            
            // Cache the updated student
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return Result<StudentResponseDto>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student with ID: {Id}", id);
            return Result<StudentResponseDto>.Fail("An error occurred while updating the student", ErrorType.Unknown);
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Deleting student with ID: {Id}", id);

            var student = await _uow.Students.GetWithUserAsync(id, ct);
            if (student is null)
            {
                _logger.LogWarning("Student with ID {Id} not found for deletion", id);
                return Result<bool>.Fail("Student not found", ErrorType.NotFound);
            }

            _uow.Students.Delete(student);
            await _uow.Users.DeleteUserAsync(student.UserId);
            await _uow.SaveChangesAsync(ct);

            // Invalidate cache
            var cacheKey = GetStudentCacheKey(id);
            await _cacheService.RemoveAsync(cacheKey);
            await InvalidateStudentListCacheAsync();
            await InvalidateStudentGroupCacheAsync(id);

            _logger.LogInformation("Successfully deleted student with ID: {Id}", id);

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student with ID: {Id}", id);
            return Result<bool>.Fail("An error occurred while deleting the student", ErrorType.Unknown);
        }
    }

    public async Task<Result<IEnumerable<StudentResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Retrieving all students");

            var cacheKey = "all_students";
            
            // Try to get from cache
            var cachedStudents = await _cacheService.GetAsync<IEnumerable<StudentResponseDto>>(cacheKey);
            if (cachedStudents != null)
            {
                _logger.LogDebug("All students retrieved from cache");
                return Result<IEnumerable<StudentResponseDto>>.Ok(cachedStudents);
            }

            var students = await _uow.Students.GetAllAsync(ct);
            var result = students.Select(s => MapToResponseDto(s, s.User)).ToList();

            // Cache the result
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            _logger.LogDebug("All students cached successfully, count: {Count}", result.Count);

            return Result<IEnumerable<StudentResponseDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all students");
            return Result<IEnumerable<StudentResponseDto>>.Fail("An error occurred while retrieving students", ErrorType.Unknown);
        }
    }

    public async Task<Result<StudentResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Setting active status to {IsActive} for student ID: {Id}", isActive, id);

            var student = await _uow.Students.GetWithUserAsync(id, ct);
            if (student is null)
            {
                _logger.LogWarning("Student with ID {Id} not found", id);
                return Result<StudentResponseDto>.Fail("Student not found", ErrorType.NotFound);
            }

            if (student.User.IsActive == isActive)
            {
                _logger.LogWarning("Student {Id} is already {Status}", id, isActive ? "active" : "inactive");
                return Result<StudentResponseDto>.Fail($"Student is already {(isActive ? "active" : "inactive")}", ErrorType.Validation);
            }

            student.User.IsActive = isActive;

            _uow.Students.Update(student);
            await _uow.Users.UpdateUserAsync(student.User);
            await _uow.SaveChangesAsync(ct);

            // Invalidate cache
            var cacheKey = GetStudentCacheKey(id);
            await _cacheService.RemoveAsync(cacheKey);
            await InvalidateStudentListCacheAsync();

            _logger.LogInformation("Successfully set active status for student ID: {Id}", id);

            var response = MapToResponseDto(student, student.User);
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return Result<StudentResponseDto>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting active status for student ID: {Id}", id);
            return Result<StudentResponseDto>.Fail("An error occurred while updating student status", ErrorType.Unknown);
        }
    }

    public async Task<Result<IEnumerable<StudentResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        try
        {
            if (groupId <= 0)
            {
                _logger.LogWarning("Invalid group ID: {GroupId}", groupId);
                return Result<IEnumerable<StudentResponseDto>>.Fail("Invalid group ID", ErrorType.Validation);
            }

            _logger.LogDebug("Retrieving students for group ID: {GroupId}", groupId);

            var group = await _uow.Groups.GetByIdAsync(groupId, ct);
            if (group is null)
            {
                _logger.LogWarning("Group with ID {GroupId} not found", groupId);
                return Result<IEnumerable<StudentResponseDto>>.Fail("Group not found", ErrorType.NotFound);
            }

            var cacheKey = $"group_students_{groupId}";
            
            var cachedStudents = await _cacheService.GetAsync<IEnumerable<StudentResponseDto>>(cacheKey);
            if (cachedStudents != null)
            {
                _logger.LogDebug("Students for group {GroupId} retrieved from cache", groupId);
                return Result<IEnumerable<StudentResponseDto>>.Ok(cachedStudents);
            }

            var students = await _uow.Students.GetByGroupIdAsync(groupId, ct);
            var result = students.Select(s => MapToResponseDto(s, s.User)).ToList();

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            _logger.LogDebug("Students for group {GroupId} cached successfully, count: {Count}", groupId, result.Count);

            return Result<IEnumerable<StudentResponseDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students for group ID: {GroupId}", groupId);
            return Result<IEnumerable<StudentResponseDto>>.Fail("An error occurred while retrieving group students", ErrorType.Unknown);
        }
    }

    public async Task<Result<StudentWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid student ID: {Id}", id);
                return Result<StudentWithGroupsDto>.Fail("Invalid student ID", ErrorType.Validation);
            }

            var cacheKey = $"student_with_groups_{id}";
            
            var cachedDto = await _cacheService.GetAsync<StudentWithGroupsDto>(cacheKey);
            if (cachedDto != null)
            {
                _logger.LogDebug("Student with groups for ID {Id} retrieved from cache", id);
                return Result<StudentWithGroupsDto>.Ok(cachedDto);
            }

            var student = await _uow.Students.GetWithGroupsAsync(id, ct);
            if (student is null)
            {
                _logger.LogWarning("Student with ID {Id} not found", id);
                return Result<StudentWithGroupsDto>.Fail("Student not found", ErrorType.NotFound);
            }

            var dto = new StudentWithGroupsDto
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
                EnrollDate = student.EnrollDate,
                Groups = student.GroupStudents
                    .Where(gs => gs.IsActive)
                    .Select(gs => new GroupShortDto
                    {
                        Id = gs.Group.Id,
                        Name = gs.Group.Name,
                        CourseName = gs.Group.Course.Name,
                        MentorName = $"{gs.Group.Mentor.User.FirstName} {gs.Group.Mentor.User.LastName}",
                        Status = gs.Group.Status
                    })
            };

            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));
            _logger.LogDebug("Student with groups for ID {Id} cached successfully", id);

            return Result<StudentWithGroupsDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student with groups for ID: {Id}", id);
            return Result<StudentWithGroupsDto>.Fail("An error occurred while retrieving student groups", ErrorType.Unknown);
        }
    }

    public async Task<Result<StudentFullProfileDto>> GetFullProfileAsync(int id, CancellationToken ct = default)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid student ID: {Id}", id);
                return Result<StudentFullProfileDto>.Fail("Invalid student ID", ErrorType.Validation);
            }

            var cacheKey = $"student_full_profile_{id}";
            
            var cachedProfile = await _cacheService.GetAsync<StudentFullProfileDto>(cacheKey);
            if (cachedProfile != null)
            {
                _logger.LogDebug("Full profile for student {Id} retrieved from cache", id);
                return Result<StudentFullProfileDto>.Ok(cachedProfile);
            }

            var student = await _uow.Students.GetFullProfileAsync(id, ct);
            if (student is null)
            {
                _logger.LogWarning("Student with ID {Id} not found", id);
                return Result<StudentFullProfileDto>.Fail("Student not found", ErrorType.NotFound);
            }

            var averageScore = await _uow.WeekResults.GetAverageScoreAsync(student.Id, 0, ct);
            var totalAbsences = student.Attendances.Count(a => !a.IsPresent);

            var dto = MapToFullProfileDto(student, averageScore, totalAbsences);

            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
            _logger.LogDebug("Full profile for student {Id} cached successfully", id);

            return Result<StudentFullProfileDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving full profile for student ID: {Id}", id);
            return Result<StudentFullProfileDto>.Fail("An error occurred while retrieving student profile", ErrorType.Unknown);
        }
    }

    public async Task<Result<StudentResponseDto>> GetMyProfileAsync(int userId, CancellationToken ct = default)
    {
        try
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Invalid user ID: {UserId}", userId);
                return Result<StudentResponseDto>.Fail("Invalid user ID", ErrorType.Validation);
            }

            _logger.LogDebug("Retrieving profile for user ID: {UserId}", userId);

            var student = await _uow.Students.GetByUserIdAsync(userId, ct);
            if (student is null)
            {
                _logger.LogWarning("Student not found for user ID: {UserId}", userId);
                return Result<StudentResponseDto>.Fail("Student not found", ErrorType.NotFound);
            }

            var user = await _uow.Users.GetUserByIdAsync(student.UserId, ct);
            if (user is null)
            {
                _logger.LogError("User not found for student with user ID: {UserId}", userId);
                return Result<StudentResponseDto>.Fail("User not found", ErrorType.NotFound);
            }

            return Result<StudentResponseDto>.Ok(MapToResponseDto(student, user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving profile for user ID: {UserId}", userId);
            return Result<StudentResponseDto>.Fail("An error occurred while retrieving profile", ErrorType.Unknown);
        }
    }

    public async Task<Result<StudentResponseDto>> UpdateMyProfileAsync(int userId, StudentUpdateDto dto, CancellationToken ct = default)
    {
        try
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Invalid user ID: {UserId}", userId);
                return Result<StudentResponseDto>.Fail("Invalid user ID", ErrorType.Validation);
            }

            _logger.LogInformation("Updating profile for user ID: {UserId}", userId);

            var student = await _uow.Students.GetByUserIdAsync(userId, ct);
            if (student is null)
            {
                _logger.LogWarning("Student not found for user ID: {UserId}", userId);
                return Result<StudentResponseDto>.Fail("Student not found", ErrorType.NotFound);
            }

            var user = await _uow.Users.GetUserByIdAsync(student.UserId, ct);
            if (user is null)
            {
                _logger.LogError("User not found for student with user ID: {UserId}", userId);
                return Result<StudentResponseDto>.Fail("User not found", ErrorType.NotFound);
            }

            if (dto.Phone is not null)
            {
                var phoneExists = await _uow.Users.PhoneExistsAsync(dto.Phone, ct);
                if (phoneExists)
                {
                    _logger.LogWarning("Phone number {Phone} already exists for another user", dto.Phone);
                    return Result<StudentResponseDto>.Fail("Phone number already exists", ErrorType.Conflict);
                }

                student.Phone = dto.Phone;
                user.PhoneNumber = dto.Phone;
            }

            if (dto.DateOfBirth.HasValue) student.DateOfBirth = dto.DateOfBirth;
            if (dto.TelegramUsername is not null) student.TelegramUsername = dto.TelegramUsername;
            if (dto.GithubUrl is not null) student.GithubUrl = dto.GithubUrl;
            if (dto.AboutMe is not null) student.AboutMe = dto.AboutMe;

            user.UpdatedAt = DateTime.UtcNow;

            _uow.Students.Update(student);
            await _uow.Users.UpdateUserAsync(user);
            await _uow.SaveChangesAsync(ct);

            // Invalidate cache
            var cacheKey = GetStudentCacheKey(student.Id);
            await _cacheService.RemoveAsync(cacheKey);
            await InvalidateStudentListCacheAsync();

            _logger.LogInformation("Successfully updated profile for user ID: {UserId}", userId);

            return Result<StudentResponseDto>.Ok(MapToResponseDto(student, user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user ID: {UserId}", userId);
            return Result<StudentResponseDto>.Fail("An error occurred while updating profile", ErrorType.Unknown);
        }
    }

    public async Task<Result<StudentFullProfileDto>> GetMyFullProfileAsync(int userId, CancellationToken ct = default)
    {
        try
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Invalid user ID: {UserId}", userId);
                return Result<StudentFullProfileDto>.Fail("Invalid user ID", ErrorType.Validation);
            }

            _logger.LogDebug("Retrieving full profile for user ID: {UserId}", userId);

            var student = await _uow.Students.GetFullProfileAsync(userId, ct);
            if (student is null)
            {
                _logger.LogWarning("Student not found for user ID: {UserId}", userId);
                return Result<StudentFullProfileDto>.Fail("Student not found", ErrorType.NotFound);
            }

            var averageScore = await _uow.WeekResults.GetAverageScoreAsync(student.Id, 0, ct);
            var totalAbsences = student.Attendances.Count(a => !a.IsPresent);

            var dto = MapToFullProfileDto(student, averageScore, totalAbsences);

            return Result<StudentFullProfileDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving full profile for user ID: {UserId}", userId);
            return Result<StudentFullProfileDto>.Fail("An error occurred while retrieving full profile", ErrorType.Unknown);
        }
    }

    #region Private Methods

    private static string GetStudentCacheKey(int id) => $"student_{id}";

    private async Task InvalidateStudentListCacheAsync()
    {
        await _cacheService.RemoveAsync("all_students");
        _logger.LogDebug("Student list cache invalidated");
    }

    private async Task InvalidateStudentGroupCacheAsync(int studentId)
    {
        var cacheKey = $"student_with_groups_{studentId}";
        await _cacheService.RemoveAsync(cacheKey);
        _logger.LogDebug("Student group cache invalidated for student ID: {StudentId}", studentId);
    }

    private static StudentResponseDto MapToResponseDto(Student student, User user) => new()
    {
        Id = student.Id,
        UserId = user.Id,
        FullName = $"{user.FirstName} {user.LastName}",
        Email = user.Email,
        Phone = student.Phone,
        AvatarUrl = user.PhotoUrl,
        TelegramUsername = student.TelegramUsername,
        GithubUrl = student.GithubUrl,
        AboutMe = student.AboutMe,
        DateOfBirth = student.DateOfBirth,
        EnrollDate = student.EnrollDate
    };

    private static StudentFullProfileDto MapToFullProfileDto(Student student, double averageScore, int totalAbsences) => new()
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
        EnrollDate = student.EnrollDate,
        Groups = student.GroupStudents
            .Where(gs => gs.IsActive)
            .Select(gs => new GroupShortDto
            {
                Id = gs.Group.Id,
                Name = gs.Group.Name,
                CourseName = gs.Group.Course.Name,
                MentorName = $"{gs.Group.Mentor.User.FirstName} {gs.Group.Mentor.User.LastName}",
                Status = gs.Group.Status
            }),
        WeekResults = student.WeekResults.Select(wr => new WeeklyResultResponseDto
        {
            Id = wr.Id,
            StudentId = wr.StudentId,
            StudentName = $"{student.User.FirstName} {student.User.LastName}",
            GroupId = wr.GroupId,
            GroupName = wr.Group.Name,
            WeekNumber = wr.WeekNumber,
            AttendanceScore = wr.AttendanceScore,
            BonusScore = wr.BonusScore,
            ExamScore = wr.ExamScore,
            TotalScore = wr.TotalScore,
            MentorComment = wr.MentorComment,
            CreatedAt = wr.CreatedAt
        }),
        AverageScore = Math.Round(averageScore, 2),
        TotalAbsences = totalAbsences
    };
   #endregion
}