using Application.Dtos.CourseDto;
using Application.Dtos.GroupDto;
using Application.Dtos.MentorDto;
using Application.Dtos.StudentDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
namespace Application.Services;

public class GroupService(IUnitOfWork uow) : IGroupService
{
    public async Task<Result<GroupResponseDto>> CreateAsync(GroupCreateDto dto, CancellationToken ct = default)
    {
        var mentorExists = await uow.Mentors.ExistsAsync(dto.MentorId, ct);
        if (!mentorExists)
            return Result<GroupResponseDto>.Fail("Mentor not found",ErrorType.NotFound);

        var courseExists = await uow.Courses.ExistsAsync(dto.CourseId, ct);
        if (!courseExists)
            return Result<GroupResponseDto>.Fail("Course not found",ErrorType.NotFound);

        var group = new Group
        {
            Name = dto.Name,
            CourseId = dto.CourseId,
            MentorId = dto.MentorId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            MaxStudents = dto.MaxStudents,
            Description = dto.Description,
            Status = GroupStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Groups.AddAsync(group, ct);
        await uow.SaveChangesAsync(ct);

        var created = await uow.Groups.GetWithDetailsAsync(group.Id, ct);

        return Result<GroupResponseDto>.Ok(MapToResponseDto(created!));
    }

    public async Task<Result<GroupResponseDto>> UpdateAsync(int id, GroupUpdateDto dto, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(id, ct);
        if (group is null)
            return Result<GroupResponseDto>.Fail("Group not found",ErrorType.NotFound);

        if (dto.MentorId.HasValue)
        {
            var mentorExists = await uow.Mentors.ExistsAsync(dto.MentorId.Value, ct);
            if (!mentorExists)
                return Result<GroupResponseDto>.Fail("Mentor not found",ErrorType.NotFound);

            group.MentorId = dto.MentorId.Value;
        }

        if (dto.Name is not null) group.Name = dto.Name;
        if (dto.EndDate.HasValue) group.EndDate = dto.EndDate;
        if (dto.MaxStudents.HasValue) group.MaxStudents = dto.MaxStudents.Value;
        if (dto.Description is not null) group.Description = dto.Description;
        if (dto.Status.HasValue) group.Status = dto.Status.Value;

        uow.Groups.Update(group);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Groups.GetWithDetailsAsync(group.Id, ct);

        return Result<GroupResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(id, ct);
        if (group is null)
            return Result<bool>.Fail("Group not found",ErrorType.NotFound);

        uow.Groups.Delete(group);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<GroupResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetWithDetailsAsync(id, ct);
        if (group is null)
            return Result<GroupResponseDto>.Fail("Group not found",ErrorType.NotFound);

        return Result<GroupResponseDto>.Ok(MapToResponseDto(group));
    }

    public async Task<Result<GroupWithStudentsDto>> GetWithStudentsAsync(int id, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetWithDetailsAsync(id, ct);
        if (group is null)
            return Result<GroupWithStudentsDto>.Fail("Group not found",ErrorType.NotFound);

        var dto = new GroupWithStudentsDto
        {
            Id = group.Id,
            Name = group.Name,
            Status = group.Status,
            Description = group.Description,
            StartDate = group.StartDate,
            EndDate = group.EndDate,
            MaxStudents = group.MaxStudents,
            CurrentStudentCount = group.GroupStudents.Count(gs => gs.IsActive),
            CreatedAt = group.CreatedAt,
            Course = new CourseResponseDto
            {
                Id = group.Course.Id,
                Name = group.Course.Name,
                Description = group.Course.Description,
                DurationWeeks = group.Course.DurationWeeks,
                IsActive = group.Course.IsActive
            },
            Mentor = new MentorResponseDto
            {
                Id = group.Mentor.Id,
                UserId = group.Mentor.UserId,
             FullName = $"{group.Mentor.User.FirstName} {group.Mentor.User.LastName}",
                Email = group.Mentor.User.Email,
                Specialization = group.Mentor.Specialization,
                Phone = group.Mentor.Phone
            },
            Students = group.GroupStudents
                .Where(gs => gs.IsActive)
                .Select(gs => new StudentResponseDto
                {
                    Id = gs.Student.Id,
                    UserId = gs.Student.UserId,
                    FullName = $"{group.Mentor.User.FirstName} {group.Mentor.User.LastName}",
                    Email = gs.Student.User.Email,
                    Phone = gs.Student.Phone,
                    AvatarUrl = gs.Student.PhotoUrl,
                    TelegramUsername = gs.Student.TelegramUsername,
                    EnrollDate = gs.Student.EnrollDate
                })
        };

        return Result<GroupWithStudentsDto>.Ok(dto);
    }

    public async Task<Result<IEnumerable<GroupResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var groups = await uow.Groups.GetAllAsync(ct);
        var result = groups.Select(MapToResponseDto);

        return Result<IEnumerable<GroupResponseDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<GroupResponseDto>>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var groups = await uow.Groups.GetAllActiveAsync(ct);
        var result = groups.Select(MapToResponseDto);

        return Result<IEnumerable<GroupResponseDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<GroupResponseDto>>> GetByMentorIdAsync(int mentorId, CancellationToken ct = default)
    {
        var mentorExists = await uow.Mentors.ExistsAsync(mentorId, ct);
        if (!mentorExists)
            return Result<IEnumerable<GroupResponseDto>>.Fail("Mentor not found",ErrorType.NotFound);

        var groups = await uow.Groups.GetByMentorIdAsync(mentorId, ct);
        var result = groups.Select(MapToResponseDto);

        return Result<IEnumerable<GroupResponseDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<GroupResponseDto>>> GetByCourseIdAsync(int courseId, CancellationToken ct = default)
    {
        var courseExists = await uow.Courses.ExistsAsync(courseId, ct);
        if (!courseExists)
            return Result<IEnumerable<GroupResponseDto>>.Fail("Course not found",ErrorType.NotFound);

        var groups = await uow.Groups.GetByCourseIdAsync(courseId, ct);
        var result = groups.Select(MapToResponseDto);

        return Result<IEnumerable<GroupResponseDto>>.Ok(result);
    }

    public async Task<Result<GroupResponseDto>> ChangeStatusAsync(int id, GroupStatus status, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(id, ct);
        if (group is null)
            return Result<GroupResponseDto>.Fail("Group not found",ErrorType.NotFound);

        group.Status = status;

        uow.Groups.Update(group);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Groups.GetWithDetailsAsync(group.Id, ct);

        return Result<GroupResponseDto>.Ok(MapToResponseDto(updated!));
    }

    // ───── Private Mapper ─────
    private static GroupResponseDto MapToResponseDto(Group group) => new()
    {
        Id = group.Id,
        Name = group.Name,
        Status = group.Status,
        Description = group.Description,
        StartDate = group.StartDate,
        EndDate = group.EndDate,
        MaxStudents = group.MaxStudents,
        CurrentStudentCount = group.GroupStudents?.Count(gs => gs.IsActive) ?? 0,
        CreatedAt = group.CreatedAt,
        Course = new CourseResponseDto
        {
            Id = group.Course.Id,
            Name = group.Course.Name,
            Description = group.Course.Description,
            DurationWeeks = group.Course.DurationWeeks,
            IsActive = group.Course.IsActive
        },
        Mentor = new MentorResponseDto
        {
            Id = group.Mentor.Id,
            UserId = group.Mentor.UserId,
            FullName = $"{group.Mentor.User.FirstName} {group.Mentor.User.LastName}",
            Email = group.Mentor.User.Email,
            Specialization = group.Mentor.Specialization,
            Phone = group.Mentor.Phone
        }
    };
}