using Application.Dtos.HomeworkDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class HomeworkService(IUnitOfWork uow) : IHomeworkService
{
    public async Task<Result<HomeworkResponseDto>> CreateAsync(HomeworkCreateDto dto, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetByIdAsync(dto.LessonId, ct);
        if (lesson is null)
            return Result<HomeworkResponseDto>.Fail("Lesson not found", ErrorType.NotFound);

        var homework = new Homework
        {
            LessonId = dto.LessonId,
            Title = dto.Title,
            Description = dto.Description,
            FileUrl = dto.FileUrl,
            Deadline = dto.Deadline,
            MaxScore = dto.MaxScore
        };

        await uow.Homeworks.AddAsync(homework, ct);
        await uow.SaveChangesAsync(ct);

        var saved = await uow.Homeworks.GetByIdAsync(homework.Id, ct);
        return Result<HomeworkResponseDto>.Ok(MapToResponseDto(saved!));
    }

    public async Task<Result<HomeworkResponseDto>> UpdateAsync(int id, HomeworkUpdateDto dto, CancellationToken ct = default)
    {
        var homework = await uow.Homeworks.GetByIdAsync(id, ct);
        if (homework is null)
            return Result<HomeworkResponseDto>.Fail("Homework not found", ErrorType.NotFound);

        homework.Title = dto.Title;
        homework.Description = dto.Description;
        homework.FileUrl = dto.FileUrl;
        homework.Deadline = dto.Deadline;
        homework.MaxScore = dto.MaxScore;

        uow.Homeworks.Update(homework);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Homeworks.GetByIdAsync(id, ct);
        return Result<HomeworkResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var homework = await uow.Homeworks.GetByIdAsync(id, ct);
        if (homework is null)
            return Result<bool>.Fail("Homework not found", ErrorType.NotFound);

        uow.Homeworks.Delete(homework);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<HomeworkResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var homework = await uow.Homeworks.GetByIdAsync(id, ct);
        if (homework is null)
            return Result<HomeworkResponseDto>.Fail("Homework not found", ErrorType.NotFound);

        return Result<HomeworkResponseDto>.Ok(MapToResponseDto(homework));
    }

    public async Task<Result<IEnumerable<HomeworkResponseDto>>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default)
    {
        var homeworks = await uow.Homeworks.GetByLessonIdAsync(lessonId, ct);
        return Result<IEnumerable<HomeworkResponseDto>>.Ok(homeworks.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<HomeworkResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var homeworks = await uow.Homeworks.GetByGroupIdAsync(groupId, ct);
        return Result<IEnumerable<HomeworkResponseDto>>.Ok(homeworks.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<HomeworkResponseDto>>> GetOverdueAsync(CancellationToken ct = default)
    {
        var homeworks = await uow.Homeworks.GetOverdueAsync(ct);
        return Result<IEnumerable<HomeworkResponseDto>>.Ok(homeworks.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<HomeworkResponseDto>>> GetMyHomeworksAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<HomeworkResponseDto>>.Fail("Student not found", ErrorType.NotFound);

        var homeworks = await uow.Homeworks.GetByGroupIdAsync(groupId, ct);
        return Result<IEnumerable<HomeworkResponseDto>>.Ok(homeworks.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<HomeworkResponseDto>>> GetMyOverdueAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<HomeworkResponseDto>>.Fail("Student not found", ErrorType.NotFound);

        var homeworks = await uow.Homeworks.GetByGroupIdAsync(groupId, ct);
        var overdue = homeworks.Where(h => h.Deadline < DateTime.UtcNow);
        return Result<IEnumerable<HomeworkResponseDto>>.Ok(overdue.Select(MapToResponseDto));
    }

    private static HomeworkResponseDto MapToResponseDto(Homework h) => new()
    {
        Id = h.Id,
        LessonId = h.LessonId,
        WeekNumber = h.Lesson?.WeekNumber ?? 0,
        LessonDate = h.Lesson?.LessonDate ?? DateTime.MinValue,
        Title = h.Title,
        Description = h.Description,
        FileUrl = h.FileUrl,
        Deadline = h.Deadline,
        MaxScore = h.MaxScore,
        SubmittedCount = h.LessonScores?.Count ?? 0,
        CheckedCount = h.LessonScores?.Count(ls => ls.Score > 0) ?? 0
    };
}