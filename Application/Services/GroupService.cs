using Application.Dtos.CourseDto;
using Application.Dtos.GroupDto;
using Application.Dtos.LessonDto;
using Application.Dtos.MentorDto;
using Application.Dtos.StudentDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class GroupService(IUnitOfWork uow) : IGroupService
{
    // ───── Basic CRUD ─────

    public async Task<Result<GroupResponseDto>> CreateAsync(GroupCreateDto dto, CancellationToken ct = default)
    {
        var mentorExists = await uow.Mentors.ExistsAsync(dto.MentorId, ct);
        if (!mentorExists)
            return Result<GroupResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

        var courseExists = await uow.Courses.ExistsAsync(dto.CourseId, ct);
        if (!courseExists)
            return Result<GroupResponseDto>.Fail("Course not found", ErrorType.NotFound);

        var group = new Group
        {
            Name = dto.Name,
            CourseId = dto.CourseId,
            MentorId = dto.MentorId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            MaxStudents = dto.MaxStudents,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Groups.AddAsync(group, ct);
        await uow.SaveChangesAsync(ct);

        var created = await uow.Groups.GetByIdAsync(group.Id, ct);
        return Result<GroupResponseDto>.Ok(MapToResponseDto(created!));
    }

    public async Task<Result<GroupResponseDto>> UpdateAsync(int id, GroupUpdateDto dto, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(id, ct);
        if (group is null)
            return Result<GroupResponseDto>.Fail("Group not found", ErrorType.NotFound);

        var mentorExists = await uow.Mentors.ExistsAsync(dto.MentorId, ct);
        if (!mentorExists)
            return Result<GroupResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

        group.Name = dto.Name;
        group.MentorId = dto.MentorId;
        group.EndDate = dto.EndDate;
        group.MaxStudents = dto.MaxStudents;

        uow.Groups.Update(group);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Groups.GetByIdAsync(group.Id, ct);
        return Result<GroupResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(id, ct);
        if (group is null)
            return Result<bool>.Fail("Group not found", ErrorType.NotFound);

        uow.Groups.Delete(group);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<GroupResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(id, ct);
        if (group is null)
            return Result<GroupResponseDto>.Fail("Group not found", ErrorType.NotFound);

        return Result<GroupResponseDto>.Ok(MapToResponseDto(group));
    }

    // ───── Queries ─────

    public async Task<Result<IEnumerable<GroupResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var groups = await uow.Groups.GetAllAsync(ct);
        return Result<IEnumerable<GroupResponseDto>>.Ok(groups.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<GroupResponseDto>>> GetActiveAsync(CancellationToken ct = default)
    {
        var groups = await uow.Groups.GetActiveAsync(ct);
        return Result<IEnumerable<GroupResponseDto>>.Ok(groups.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<GroupResponseDto>>> GetByStatusAsync(GroupStatus status, CancellationToken ct = default)
    {
        var groups = await uow.Groups.GetByStatusAsync(status, ct);
        return Result<IEnumerable<GroupResponseDto>>.Ok(groups.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<GroupResponseDto>>> GetByCourseIdAsync(int courseId, CancellationToken ct = default)
    {
        var groups = await uow.Groups.GetByCourseIdAsync(courseId, ct);
        return Result<IEnumerable<GroupResponseDto>>.Ok(groups.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<GroupResponseDto>>> GetByMentorIdAsync(int mentorId, CancellationToken ct = default)
    {
        var groups = await uow.Groups.GetByMentorIdAsync(mentorId, ct);
        return Result<IEnumerable<GroupResponseDto>>.Ok(groups.Select(MapToResponseDto));
    }

    // ───── Detailed ─────

    public async Task<Result<GroupWithStudentsDto>> GetWithStudentsAsync(int id, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetWithStudentsAsync(id, ct);
        if (group is null)
            return Result<GroupWithStudentsDto>.Fail("Group not found", ErrorType.NotFound);

        return Result<GroupWithStudentsDto>.Ok(MapToWithStudentsDto(group));
    }

    public async Task<Result<GroupWithLessonsDto>> GetWithLessonsAsync(int id, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetWithLessonsAsync(id, ct);
        if (group is null)
            return Result<GroupWithLessonsDto>.Fail("Group not found", ErrorType.NotFound);

        return Result<GroupWithLessonsDto>.Ok(MapToWithLessonsDto(group));
    }

    public async Task<Result<GroupFullDto>> GetFullAsync(int id, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetWithStudentsAsync(id, ct);
        if (group is null)
            return Result<GroupFullDto>.Fail("Group not found", ErrorType.NotFound);

        var groupWithLessons = await uow.Groups.GetWithLessonsAsync(id, ct);

        return Result<GroupFullDto>.Ok(new GroupFullDto
        {
            Id = group.Id,
            Name = group.Name,
            CourseName = group.Course?.Name ?? "",
            MentorName = group.Mentor?.User != null
                ? $"{group.Mentor.User.FirstName} {group.Mentor.User.LastName}"
                : "",
            StartDate = group.StartDate,
            EndDate = group.EndDate,
            MaxStudents = group.MaxStudents,
            ActiveStudents = group.GroupStudents.Count(gs => gs.IsActive),
            Status = group.Status.ToString(),
            CreatedAt = group.CreatedAt,
            Students = group.GroupStudents.Select(gs => new GroupStudentDto
            {
                Id = gs.Id,
                StudentId = gs.StudentId,
                StudentName = gs.Student?.User != null
                    ? $"{gs.Student.User.FirstName} {gs.Student.User.LastName}"
                    : "",
                Email = gs.Student?.User?.Email,
                PhotoUrl = gs.Student?.PhotoUrl,
                JoinedAt = gs.JoinedAt,
                IsActive = gs.IsActive
            }),
            Lessons = groupWithLessons?.Lessons.Select(l => new LessonShortDto
            {
                Id = l.Id,
                WeekNumber = l.WeekNumber,
                LessonDate = l.LessonDate,
                Title = l.Title,
                IsCompleted = l.IsCompleted
            }) ?? Enumerable.Empty<LessonShortDto>()
        });
    }

    // ───── Admin Actions ─────

    public async Task<Result<GroupResponseDto>> ChangeStatusAsync(int id, GroupStatus status, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(id, ct);
        if (group is null)
            return Result<GroupResponseDto>.Fail("Group not found", ErrorType.NotFound);

        group.Status = status;
        uow.Groups.Update(group);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Groups.GetByIdAsync(id, ct);
        return Result<GroupResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<GroupResponseDto>> ChangeMentorAsync(int id, int mentorId, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(id, ct);
        if (group is null)
            return Result<GroupResponseDto>.Fail("Group not found", ErrorType.NotFound);

        var mentorExists = await uow.Mentors.ExistsAsync(mentorId, ct);
        if (!mentorExists)
            return Result<GroupResponseDto>.Fail("Mentor not found", ErrorType.NotFound);

        group.MentorId = mentorId;
        uow.Groups.Update(group);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Groups.GetByIdAsync(id, ct);
        return Result<GroupResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<int>> GetActiveStudentCountAsync(int id, CancellationToken ct = default)
    {
        var count = await uow.Groups.GetStudentCountAsync(id, ct);
        return Result<int>.Ok(count);
    }

    public async Task<Result<bool>> HasAvailableSeatsAsync(int id, CancellationToken ct = default)
    {
        var hasSeats = await uow.Groups.HasAvailableSeatsAsync(id, ct);
        return Result<bool>.Ok(hasSeats);
    }

    // ───── Lookup ─────

    public async Task<Result<IEnumerable<GroupShortDto>>> GetLookupAsync(CancellationToken ct = default)
    {
        var groups = await uow.Groups.GetAllAsync(ct);
        return Result<IEnumerable<GroupShortDto>>.Ok(groups.Select(MapToShortDto));
    }

    public async Task<Result<IEnumerable<GroupShortDto>>> GetActiveLookupAsync(CancellationToken ct = default)
    {
        var groups = await uow.Groups.GetActiveAsync(ct);
        return Result<IEnumerable<GroupShortDto>>.Ok(groups.Select(MapToShortDto));
    }

    public async Task<Result<IEnumerable<GroupShortDto>>> GetLookupByCourseIdAsync(int courseId, CancellationToken ct = default)
    {
        var groups = await uow.Groups.GetByCourseIdAsync(courseId, ct);
        return Result<IEnumerable<GroupShortDto>>.Ok(groups.Select(MapToShortDto));
    }

    public async Task<Result<IEnumerable<GroupShortDto>>> GetLookupByMentorIdAsync(int mentorId, CancellationToken ct = default)
    {
        var groups = await uow.Groups.GetByMentorIdAsync(mentorId, ct);
        return Result<IEnumerable<GroupShortDto>>.Ok(groups.Select(MapToShortDto));
    }

    // ───── Mappers ─────

    private static GroupResponseDto MapToResponseDto(Group g) => new()
    {
        Id = g.Id,
        Name = g.Name,
        CourseId = g.CourseId,
        CourseName = g.Course?.Name ?? "",
        MentorId = g.MentorId,
        MentorName = g.Mentor?.User != null
            ? $"{g.Mentor.User.FirstName} {g.Mentor.User.LastName}"
            : "",
        StartDate = g.StartDate,
        EndDate = g.EndDate,
        MaxStudents = g.MaxStudents,
        ActiveStudents = g.GroupStudents?.Count(gs => gs.IsActive) ?? 0,
        Status = g.Status.ToString(),
        CreatedAt = g.CreatedAt
    };

    private static GroupShortDto MapToShortDto(Group g) => new()
    {
        Id = g.Id,
        Name = g.Name,
        CourseName = g.Course?.Name ?? "",
        MentorName = g.Mentor?.User != null
            ? $"{g.Mentor.User.FirstName} {g.Mentor.User.LastName}"
            : "",
        Status = g.Status.ToString(),
        StudentCount = g.GroupStudents?.Count(gs => gs.IsActive) ?? 0,
        StartDate = g.StartDate
    };

    private static GroupWithStudentsDto MapToWithStudentsDto(Group g) => new()
    {
        Id = g.Id,
        Name = g.Name,
        CourseName = g.Course?.Name ?? "",
        MentorName = g.Mentor?.User != null
            ? $"{g.Mentor.User.FirstName} {g.Mentor.User.LastName}"
            : "",
        StartDate = g.StartDate,
        EndDate = g.EndDate,
        MaxStudents = g.MaxStudents,
        Status = g.Status.ToString(),
        Students = g.GroupStudents.Where(gs => gs.IsActive).Select(gs => new GroupStudentDto
        {
            Id = gs.Id,
            StudentId = gs.StudentId,
            StudentName = gs.Student?.User != null
                ? $"{gs.Student.User.FirstName} {gs.Student.User.LastName}"
                : "",
            Email = gs.Student?.User?.Email,
            PhotoUrl = gs.Student?.PhotoUrl,
            JoinedAt = gs.JoinedAt,
            IsActive = gs.IsActive
        })
    };

    private static GroupWithLessonsDto MapToWithLessonsDto(Group g) => new()
    {
        Id = g.Id,
        Name = g.Name,
        CourseName = g.Course?.Name ?? "",
        MentorName = g.Mentor?.User != null
            ? $"{g.Mentor.User.FirstName} {g.Mentor.User.LastName}"
            : "",
        StartDate = g.StartDate,
        EndDate = g.EndDate,
        Status = g.Status.ToString(),
        Lessons = g.Lessons.Select(l => new LessonShortDto
        {
            Id = l.Id,
            WeekNumber = l.WeekNumber,
            LessonDate = l.LessonDate,
            Title = l.Title,
            IsCompleted = l.IsCompleted
        })
    };
}