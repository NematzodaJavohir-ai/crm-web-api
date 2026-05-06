using Application.Dtos.AttendanceDto;
using Application.Dtos.LessonDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
namespace Application.Services;

public class LessonService(IUnitOfWork uow) : ILessonService
{
    public async Task<Result<LessonResponseDto>> CreateAsync(LessonCreateDto dto, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(dto.GroupId, ct);
        if (group is null)
            return Result<LessonResponseDto>.Fail("Group not found", ErrorType.NotFound);

        if (group.Status != GroupStatus.Active)
            return Result<LessonResponseDto>.Fail("Cannot add lesson to inactive group", ErrorType.Validation);

        var dateExists = await uow.Lessons.DateExistsInGroupAsync(dto.GroupId, dto.LessonDate, ct);
        if (dateExists)
            return Result<LessonResponseDto>.Fail("Lesson already exists on this date for this group", ErrorType.Conflict);

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
        if (dto.IsCompleted.HasValue) lesson.IsCompleted = dto.IsCompleted.Value;

        uow.Lessons.Update(lesson);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Lessons.GetByIdAsync(id, ct);

        return Result<LessonResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetWithAttendancesAsync(id, ct);
        if (lesson is null)
            return Result<bool>.Fail("Lesson not found", ErrorType.NotFound);

        if (lesson.Attendances.Any())
            return Result<bool>.Fail("Cannot delete lesson with attendance records", ErrorType.Validation);

        uow.Lessons.Delete(lesson);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<LessonResponseDto>> MarkAsCompletedAsync(int id, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetByIdAsync(id, ct);
        if (lesson is null)
            return Result<LessonResponseDto>.Fail("Lesson not found", ErrorType.NotFound);

        if (lesson.IsCompleted)
            return Result<LessonResponseDto>.Fail("Lesson is already completed", ErrorType.Validation);

        lesson.IsCompleted = true;

        uow.Lessons.Update(lesson);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Lessons.GetByIdAsync(id, ct);

        return Result<LessonResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<IEnumerable<LessonResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(groupId, ct);
        if (group is null)
            return Result<IEnumerable<LessonResponseDto>>.Fail("Group not found", ErrorType.NotFound);

        var lessons = await uow.Lessons.GetByGroupIdAsync(groupId, ct);
        var result = lessons.Select(MapToResponseDto);

        return Result<IEnumerable<LessonResponseDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<LessonResponseDto>>> GetByWeekNumberAsync(int groupId, int weekNumber, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(groupId, ct);
        if (group is null)
            return Result<IEnumerable<LessonResponseDto>>.Fail("Group not found", ErrorType.NotFound);

        var lessons = await uow.Lessons.GetByWeekNumberAsync(groupId, weekNumber, ct);
        var result = lessons.Select(MapToResponseDto);

        return Result<IEnumerable<LessonResponseDto>>.Ok(result);
    }

    public async Task<Result<LessonWithAttendancesDto>> GetWithAttendancesAsync(int id, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetWithAttendancesAsync(id, ct);
        if (lesson is null)
            return Result<LessonWithAttendancesDto>.Fail("Lesson not found", ErrorType.NotFound);

        var dto = new LessonWithAttendancesDto
        {
            Id = lesson.Id,
            GroupId = lesson.GroupId,
            GroupName = lesson.Group.Name,
            WeekNumber = lesson.WeekNumber,
            LessonDate = lesson.LessonDate,
            Title = lesson.Title,
            Description = lesson.Description,
            HomeworkDescription = lesson.HomeworkDescription,
            MaterialUrl = lesson.MaterialUrl,
            IsCompleted = lesson.IsCompleted,
            CreatedAt = lesson.CreatedAt,
            Attendances = lesson.Attendances.Select(a => new AttendanceResponseDto
            {
                Id = a.Id,
                LessonId = a.LessonId,
                LessonDate = lesson.LessonDate,
                WeekNumber = lesson.WeekNumber,
                StudentId = a.StudentId,
                StudentName = $"{a.Student.User.FirstName} {a.Student.User.LastName}",
                IsPresent = a.IsPresent,
                Score = a.Score,
                AbsenceReason = a.AbsenceReason,
                MentorNote = a.MentorNote,
                HomeworkDone = a.HomeworkDone,
                HomeworkScore = a.HomeworkScore,
                CreatedAt = a.CreatedAt
            })
        };

        return Result<LessonWithAttendancesDto>.Ok(dto);
    }

    public async Task<Result<IEnumerable<LessonResponseDto>>> GetMyLessonsAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<LessonResponseDto>>.Fail("Student not found", ErrorType.NotFound);
        var isInGroup = await uow.GroupStudents.IsStudentInGroupAsync(groupId, student.Id, ct);
        if (!isInGroup)
            return Result<IEnumerable<LessonResponseDto>>.Fail("Student is not in this group", ErrorType.Validation);
        var lessons = await uow.Lessons.GetByGroupIdAsync(groupId, ct);
        var result = lessons.Select(MapToResponseDto);
        return Result<IEnumerable<LessonResponseDto>>.Ok(result);
    }
    private static LessonResponseDto MapToResponseDto(Lesson lesson) => new()
    {
        Id = lesson.Id,
        GroupId = lesson.GroupId,
        GroupName = lesson.Group.Name,
        WeekNumber = lesson.WeekNumber,
        LessonDate = lesson.LessonDate,
        Title = lesson.Title,
        Description = lesson.Description,
        HomeworkDescription = lesson.HomeworkDescription,
        MaterialUrl = lesson.MaterialUrl,
        IsCompleted = lesson.IsCompleted,
        CreatedAt = lesson.CreatedAt
    };
}