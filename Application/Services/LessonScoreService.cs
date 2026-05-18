using Application.Dtos.LessonScoreDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class LessonScoreService(IUnitOfWork uow) : ILessonScoreService
{
    public async Task<Result<LessonScoreResponseDto>> SubmitAsync(LessonScoreCreateDto dto, CancellationToken ct = default)
    {
        var homework = await uow.Homeworks.GetByIdAsync(dto.HomeworkId, ct);
        if (homework is null)
            return Result<LessonScoreResponseDto>.Fail("Homework not found", ErrorType.NotFound);

        var student = await uow.Students.GetByIdAsync(dto.StudentId, ct);
        if (student is null)
            return Result<LessonScoreResponseDto>.Fail("Student not found", ErrorType.NotFound);

        var existing = await uow.LessonScores.GetByHomeworkAndStudentAsync(dto.HomeworkId, dto.StudentId, ct);
        if (existing is not null)
            return Result<LessonScoreResponseDto>.Fail("Submission already exists", ErrorType.Conflict);

        var lessonScore = new LessonScore
        {
            HomeworkId = dto.HomeworkId,
            StudentId = dto.StudentId,
            SubmissionUrl = dto.SubmissionUrl,
            Score = 0,
            SubmittedAt = DateTime.UtcNow
        };

        await uow.LessonScores.AddAsync(lessonScore, ct);
        await uow.SaveChangesAsync(ct);

        var saved = await uow.LessonScores.GetByIdAsync(lessonScore.Id, ct);
        return Result<LessonScoreResponseDto>.Ok(MapToResponseDto(saved!));
    }

    public async Task<Result<LessonScoreResponseDto>> UpdateAsync(int id, LessonScoreUpdateDto dto, CancellationToken ct = default)
    {
        var lessonScore = await uow.LessonScores.GetByIdAsync(id, ct);
        if (lessonScore is null)
            return Result<LessonScoreResponseDto>.Fail("Submission not found", ErrorType.NotFound);

        if (lessonScore.Score > 0)
            return Result<LessonScoreResponseDto>.Fail("Cannot update checked submission", ErrorType.Validation);

        if (dto.SubmissionUrl is not null) lessonScore.SubmissionUrl = dto.SubmissionUrl;

        uow.LessonScores.Update(lessonScore);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.LessonScores.GetByIdAsync(id, ct);
        return Result<LessonScoreResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var lessonScore = await uow.LessonScores.GetByIdAsync(id, ct);
        if (lessonScore is null)
            return Result<bool>.Fail("Submission not found", ErrorType.NotFound);

        uow.LessonScores.Delete(lessonScore);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<LessonScoreResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var lessonScore = await uow.LessonScores.GetByIdAsync(id, ct);
        if (lessonScore is null)
            return Result<LessonScoreResponseDto>.Fail("Submission not found", ErrorType.NotFound);

        return Result<LessonScoreResponseDto>.Ok(MapToResponseDto(lessonScore));
    }

    public async Task<Result<LessonScoreResponseDto>> CheckAsync(int id, int score, string? feedback, CancellationToken ct = default)
    {
        var lessonScore = await uow.LessonScores.GetByIdAsync(id, ct);
        if (lessonScore is null)
            return Result<LessonScoreResponseDto>.Fail("Submission not found", ErrorType.NotFound);

        lessonScore.Score = Math.Clamp(score, 0, lessonScore.Homework.MaxScore);
        lessonScore.MentorFeedback = feedback;

        uow.LessonScores.Update(lessonScore);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.LessonScores.GetByIdAsync(id, ct);
        return Result<LessonScoreResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<IEnumerable<LessonScoreResponseDto>>> GetByHomeworkIdAsync(int homeworkId, CancellationToken ct = default)
    {
        var scores = await uow.LessonScores.GetByHomeworkIdAsync(homeworkId, ct);
        return Result<IEnumerable<LessonScoreResponseDto>>.Ok(scores.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<LessonScoreResponseDto>>> GetUncheckedByHomeworkIdAsync(int homeworkId, CancellationToken ct = default)
    {
        var scores = await uow.LessonScores.GetByHomeworkIdAsync(homeworkId, ct);
        return Result<IEnumerable<LessonScoreResponseDto>>.Ok(scores.Where(s => s.Score == 0).Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<LessonScoreResponseDto>>> GetMyScoresAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<LessonScoreResponseDto>>.Fail("Student not found", ErrorType.NotFound);

        var scores = await uow.LessonScores.GetByStudentIdAsync(student.Id, ct);
        var filtered = scores.Where(s => s.Homework?.Lesson?.GroupId == groupId);
        return Result<IEnumerable<LessonScoreResponseDto>>.Ok(filtered.Select(MapToResponseDto));
    }

    public async Task<Result<double>> GetMyAverageScoreAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<double>.Fail("Student not found", ErrorType.NotFound);

        var avg = await uow.LessonScores.GetAverageScoreByStudentAndGroupAsync(student.Id, groupId, ct);
        return Result<double>.Ok(Math.Round(avg, 2));
    }

    public async Task<Result<int>> GetMyTotalScoreAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<int>.Fail("Student not found", ErrorType.NotFound);

        var total = await uow.LessonScores.GetTotalScoreByStudentAndGroupAsync(student.Id, groupId, ct);
        return Result<int>.Ok(total);
    }

    private static LessonScoreResponseDto MapToResponseDto(LessonScore ls) => new()
    {
        Id = ls.Id,
        HomeworkId = ls.HomeworkId,
        HomeworkTitle = ls.Homework?.Title ?? "",
        StudentId = ls.StudentId,
        StudentName = ls.Student?.User != null
            ? $"{ls.Student.User.FirstName} {ls.Student.User.LastName}"
            : "",
        Score = ls.Score,
        SubmissionUrl = ls.SubmissionUrl,
        MentorFeedback = ls.MentorFeedback,
        SubmittedAt = ls.SubmittedAt
    };
}