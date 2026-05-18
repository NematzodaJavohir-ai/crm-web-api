using Application.Dtos.AttendanceDto;
using Application.Dtos.LessonDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class LessonService(IUnitOfWork uow) : ILessonService
{
    // ───── Basic CRUD ─────

    public async Task<Result<LessonResponseDto>> CreateAsync(LessonCreateDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result<LessonResponseDto>.Fail("Lesson title is required", ErrorType.Validation);

        var group = await uow.Groups.GetByIdAsync(dto.GroupId, ct);
        if (group is null)
            return Result<LessonResponseDto>.Fail("Group not found", ErrorType.NotFound);

        if (group.Status != GroupStatus.Active)
            return Result<LessonResponseDto>.Fail("Cannot add lesson to inactive group", ErrorType.Validation);

        if (dto.LessonDate < group.StartDate)
            return Result<LessonResponseDto>.Fail("Lesson date cannot be before group start date", ErrorType.Validation);

        if (group.EndDate.HasValue && dto.LessonDate > group.EndDate.Value)
            return Result<LessonResponseDto>.Fail("Lesson date cannot be after group end date", ErrorType.Validation);

        var lesson = new Lesson
        {
            GroupId = dto.GroupId,
            WeekNumber = dto.WeekNumber,
            LessonDate = dto.LessonDate,
            Title = dto.Title,
            Description = dto.Description,
            HomeworkDescription = dto.HomeworkDescription,
            MaterialUrl = dto.MaterialUrl,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Lessons.AddAsync(lesson, ct);
        await uow.SaveChangesAsync(ct);

        var saved = await uow.Lessons.GetByIdAsync(lesson.Id, ct);
        return Result<LessonResponseDto>.Ok(MapToResponseDto(saved!));
    }

    public async Task<Result<LessonResponseDto>> UpdateAsync(int id, LessonUpdateDto dto, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetByIdAsync(id, ct);
        if (lesson is null)
            return Result<LessonResponseDto>.Fail("Lesson not found", ErrorType.NotFound);

        if (lesson.IsCompleted)
            return Result<LessonResponseDto>.Fail("Cannot update completed lesson", ErrorType.Validation);

        if (dto.Title is not null) lesson.Title = dto.Title;
        if (dto.Description is not null) lesson.Description = dto.Description;
        if (dto.HomeworkDescription is not null) lesson.HomeworkDescription = dto.HomeworkDescription;
        if (dto.MaterialUrl is not null) lesson.MaterialUrl = dto.MaterialUrl;

        uow.Lessons.Update(lesson);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Lessons.GetByIdAsync(id, ct);
        return Result<LessonResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetByIdAsync(id, ct);
        if (lesson is null)
            return Result<bool>.Fail("Lesson not found", ErrorType.NotFound);

        uow.Lessons.Delete(lesson);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<LessonResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetByIdAsync(id, ct);
        if (lesson is null)
            return Result<LessonResponseDto>.Fail("Lesson not found", ErrorType.NotFound);

        return Result<LessonResponseDto>.Ok(MapToResponseDto(lesson));
    }

    // ───── Bulk Operations ─────

    public async Task<Result<IEnumerable<LessonResponseDto>>> BulkCreateAsync(BulkCreateLessonsDto dto, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(dto.GroupId, ct);
        if (group is null)
            return Result<IEnumerable<LessonResponseDto>>.Fail("Group not found", ErrorType.NotFound);

        var lessons = new List<Lesson>();
        foreach (var item in dto.Lessons)
        {
            lessons.Add(new Lesson
            {
                GroupId = dto.GroupId,
                WeekNumber = item.WeekNumber,
                LessonDate = item.LessonDate,
                Title = item.Title,
                Description = item.Description,
                CreatedAt = DateTime.UtcNow
            });
        }

        foreach (var lesson in lessons)
            await uow.Lessons.AddAsync(lesson, ct);

        await uow.SaveChangesAsync(ct);
        return Result<IEnumerable<LessonResponseDto>>.Ok(lessons.Select(MapToResponseDto));
    }

    public async Task<Result<bool>> BulkDeleteAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        foreach (var id in ids)
        {
            var lesson = await uow.Lessons.GetByIdAsync(id, ct);
            if (lesson is not null)
                uow.Lessons.Delete(lesson);
        }
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    // ───── Queries ─────

    public async Task<Result<IEnumerable<LessonResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var lessons = await uow.Lessons.GetAllAsync(ct);
        return Result<IEnumerable<LessonResponseDto>>.Ok(lessons.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<LessonResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var lessons = await uow.Lessons.GetByGroupIdAsync(groupId, ct);
        return Result<IEnumerable<LessonResponseDto>>.Ok(lessons.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<LessonResponseDto>>> GetByWeekNumberAsync(int groupId, int weekNumber, CancellationToken ct = default)
    {
        var lessons = await uow.Lessons.GetByWeekAsync(groupId, weekNumber, ct);
        return Result<IEnumerable<LessonResponseDto>>.Ok(lessons.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<LessonResponseDto>>> GetByDateRangeAsync(int groupId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var lessons = await uow.Lessons.GetByDateRangeAsync(groupId, from, to, ct);
        return Result<IEnumerable<LessonResponseDto>>.Ok(lessons.Select(MapToResponseDto));
    }

    // ───── Detailed ─────

    public async Task<Result<LessonWithAttendancesDto>> GetWithAttendancesAsync(int id, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetWithAttendanceAsync(id, ct);
        if (lesson is null)
            return Result<LessonWithAttendancesDto>.Fail("Lesson not found", ErrorType.NotFound);

        return Result<LessonWithAttendancesDto>.Ok(new LessonWithAttendancesDto
        {
            Id = lesson.Id,
            GroupId = lesson.GroupId,
            GroupName = lesson.Group?.Name ?? "",
            WeekNumber = lesson.WeekNumber,
            LessonDate = lesson.LessonDate,
            Title = lesson.Title,
            Description = lesson.Description,
            MaterialUrl = lesson.MaterialUrl,
            IsCompleted = lesson.IsCompleted,
            PresentCount = lesson.Attendances.Count(a => a.IsPresent),
            AbsentCount = lesson.Attendances.Count(a => !a.IsPresent),
            Attendances = lesson.Attendances.Select(a => new AttendanceShortDto
            {
                Id = a.Id,
                StudentId = a.StudentId,
                StudentName = a.Student?.User != null
                    ? $"{a.Student.User.FirstName} {a.Student.User.LastName}"
                    : "",
                IsPresent = a.IsPresent,
                MentorNote = a.MentorNote
            })
        });
    }

    public async Task<Result<LessonWithHomeworksDto>> GetWithHomeworksAsync(int id, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetByIdAsync(id, ct);
        if (lesson is null)
            return Result<LessonWithHomeworksDto>.Fail("Lesson not found", ErrorType.NotFound);

        return Result<LessonWithHomeworksDto>.Ok(new LessonWithHomeworksDto
        {
            Id = lesson.Id,
            WeekNumber = lesson.WeekNumber,
            LessonDate = lesson.LessonDate,
            Title = lesson.Title,
            IsCompleted = lesson.IsCompleted,
            Homeworks = lesson.Homeworks.Select(h => new HomeworkResponseDto
            {
                Id = h.Id,
                LessonId = h.LessonId,
                WeekNumber = lesson.WeekNumber,
                LessonDate = lesson.LessonDate,
                Title = h.Title,
                Description = h.Description,
                FileUrl = h.FileUrl,
                Deadline = h.Deadline,
                MaxScore = h.MaxScore
            })
        });
    }

    public async Task<Result<LessonFullDto>> GetFullAsync(int id, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetWithAttendanceAsync(id, ct);
        if (lesson is null)
            return Result<LessonFullDto>.Fail("Lesson not found", ErrorType.NotFound);

        return Result<LessonFullDto>.Ok(new LessonFullDto
        {
            Id = lesson.Id,
            GroupId = lesson.GroupId,
            GroupName = lesson.Group?.Name ?? "",
            CourseName = lesson.Group?.Course?.Name ?? "",
            MentorName = lesson.Group?.Mentor?.User != null
                ? $"{lesson.Group.Mentor.User.FirstName} {lesson.Group.Mentor.User.LastName}"
                : "",
            WeekNumber = lesson.WeekNumber,
            LessonDate = lesson.LessonDate,
            Title = lesson.Title,
            Description = lesson.Description,
            HomeworkDescription = lesson.HomeworkDescription,
            MaterialUrl = lesson.MaterialUrl,
            IsCompleted = lesson.IsCompleted,
            PresentCount = lesson.Attendances.Count(a => a.IsPresent),
            AbsentCount = lesson.Attendances.Count(a => !a.IsPresent),
            CreatedAt = lesson.CreatedAt,
            Attendances = lesson.Attendances.Select(a => new AttendanceShortDto
            {
                Id = a.Id,
                StudentId = a.StudentId,
                StudentName = a.Student?.User != null
                    ? $"{a.Student.User.FirstName} {a.Student.User.LastName}"
                    : "",
                IsPresent = a.IsPresent,
                MentorNote = a.MentorNote
            }),
            Homeworks = lesson.Homeworks.Select(h => new HomeworkShortDto
            {
                Id = h.Id,
                Title = h.Title,
                Deadline = h.Deadline,
                MaxScore = h.MaxScore
            })
        });
    }

    // ───── Actions ─────

    public async Task<Result<LessonResponseDto>> MarkAsCompletedAsync(int id, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetByIdAsync(id, ct);
        if (lesson is null)
            return Result<LessonResponseDto>.Fail("Lesson not found", ErrorType.NotFound);

        lesson.IsCompleted = true;
        uow.Lessons.Update(lesson);
        await uow.SaveChangesAsync(ct);

        return Result<LessonResponseDto>.Ok(MapToResponseDto(lesson));
    }

    public async Task<Result<LessonResponseDto>> MarkAsNotCompletedAsync(int id, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetByIdAsync(id, ct);
        if (lesson is null)
            return Result<LessonResponseDto>.Fail("Lesson not found", ErrorType.NotFound);

        lesson.IsCompleted = false;
        uow.Lessons.Update(lesson);
        await uow.SaveChangesAsync(ct);

        return Result<LessonResponseDto>.Ok(MapToResponseDto(lesson));
    }

    public async Task<Result<LessonResponseDto>> CompleteLessonAsync(int id, CompleteLessonDto dto, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetByIdAsync(id, ct);
        if (lesson is null)
            return Result<LessonResponseDto>.Fail("Lesson not found", ErrorType.NotFound);

        lesson.IsCompleted = dto.IsCompleted;
        uow.Lessons.Update(lesson);
        await uow.SaveChangesAsync(ct);

        return Result<LessonResponseDto>.Ok(MapToResponseDto(lesson));
    }

    // ───── Stats ─────

    public async Task<Result<int>> GetCompletedCountAsync(int groupId, CancellationToken ct = default)
    {
        var count = await uow.Lessons.GetCompletedCountAsync(groupId, ct);
        return Result<int>.Ok(count);
    }

    public async Task<Result<int>> GetTotalCountAsync(int groupId, CancellationToken ct = default)
    {
        var lessons = await uow.Lessons.GetByGroupIdAsync(groupId, ct);
        return Result<int>.Ok(lessons.Count());
    }

    public async Task<Result<LessonResponseDto>> GetNextLessonAsync(int groupId, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetNextLessonAsync(groupId, ct);
        if (lesson is null)
            return Result<LessonResponseDto>.Fail("No upcoming lessons", ErrorType.NotFound);

        return Result<LessonResponseDto>.Ok(MapToResponseDto(lesson));
    }

    public async Task<Result<LessonResponseDto>> GetPreviousLessonAsync(int groupId, CancellationToken ct = default)
    {
        var lessons = await uow.Lessons.GetByGroupIdAsync(groupId, ct);
        var previous = lessons
            .Where(l => l.LessonDate < DateTime.UtcNow)
            .OrderByDescending(l => l.LessonDate)
            .FirstOrDefault();

        if (previous is null)
            return Result<LessonResponseDto>.Fail("No previous lessons", ErrorType.NotFound);

        return Result<LessonResponseDto>.Ok(MapToResponseDto(previous));
    }

    // ───── Student ─────

    public async Task<Result<IEnumerable<LessonResponseDto>>> GetMyLessonsAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<LessonResponseDto>>.Fail("Student not found", ErrorType.NotFound);

        var lessons = await uow.Lessons.GetByGroupIdAsync(groupId, ct);
        return Result<IEnumerable<LessonResponseDto>>.Ok(lessons.Select(MapToResponseDto));
    }

    public async Task<Result<int>> GetMyCompletedCountAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<int>.Fail("Student not found", ErrorType.NotFound);

        var count = await uow.Lessons.GetCompletedCountAsync(groupId, ct);
        return Result<int>.Ok(count);
    }

    public async Task<Result<LessonResponseDto>> GetMyNextLessonAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<LessonResponseDto>.Fail("Student not found", ErrorType.NotFound);

        var lesson = await uow.Lessons.GetNextLessonAsync(groupId, ct);
        if (lesson is null)
            return Result<LessonResponseDto>.Fail("No upcoming lessons", ErrorType.NotFound);

        return Result<LessonResponseDto>.Ok(MapToResponseDto(lesson));
    }

    // ───── Mapper ─────

    private static LessonResponseDto MapToResponseDto(Lesson lesson) => new()
    {
        Id = lesson.Id,
        GroupId = lesson.GroupId,
        GroupName = lesson.Group?.Name ?? "",
        WeekNumber = lesson.WeekNumber,
        LessonDate = lesson.LessonDate,
        Title = lesson.Title ?? "",
        Description = lesson.Description,
        HomeworkDescription = lesson.HomeworkDescription,
        MaterialUrl = lesson.MaterialUrl,
        IsCompleted = lesson.IsCompleted,
        AttendanceCount = lesson.Attendances?.Count ?? 0,
        CreatedAt = lesson.CreatedAt
    };
}