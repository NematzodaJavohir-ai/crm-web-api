using Application.Dtos.WeeklyResultDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
namespace Application.Services;
public class WeekResultService(IUnitOfWork uow) : IWeekResultService
{
    public async Task<Result<WeeklyResultResponseDto>> CreateAsync(WeeklyResultCreateDto dto, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(dto.GroupId, ct);
        if (group is null)
            return Result<WeeklyResultResponseDto>.Fail("Group not found", ErrorType.NotFound);

        if (group.Status != GroupStatus.Active)
            return Result<WeeklyResultResponseDto>.Fail("Group is not active", ErrorType.Validation);

        var student = await uow.Students.GetByIdAsync(dto.StudentId, ct);
        if (student is null)
            return Result<WeeklyResultResponseDto>.Fail("Student not found", ErrorType.NotFound);

        var isInGroup = await uow.GroupStudents.IsStudentInGroupAsync(dto.GroupId, dto.StudentId, ct);
        if (!isInGroup)
            return Result<WeeklyResultResponseDto>.Fail("Student is not in this group", ErrorType.Validation);

        var alreadyExists = await uow.WeekResults.GetByStudentGroupWeekAsync(dto.StudentId, dto.GroupId, dto.WeekNumber, ct);
        if (alreadyExists is not null)
            return Result<WeeklyResultResponseDto>.Fail("Week result already exists for this student", ErrorType.Conflict);

        var attendanceScore = await uow.Attendances.GetTotalScoreByWeekAsync(dto.StudentId, dto.GroupId, dto.WeekNumber, ct);

        var weekResult = new WeekResult
        {
            StudentId = dto.StudentId,
            GroupId = dto.GroupId,
            WeekNumber = dto.WeekNumber,
            BonusScore = dto.BonusScore,
            ExamScore = dto.ExamScore,
            AttendanceScore = attendanceScore,
            TotalScore = attendanceScore + dto.BonusScore + dto.ExamScore,
            MentorComment = dto.MentorComment,
            CreatedAt = DateTime.UtcNow
        };

        await uow.WeekResults.AddAsync(weekResult, ct);
        await uow.SaveChangesAsync(ct);

        var saved = await uow.WeekResults.GetByStudentGroupWeekAsync(dto.StudentId, dto.GroupId, dto.WeekNumber, ct);

        return Result<WeeklyResultResponseDto>.Ok(MapToResponseDto(saved!));
    }

    public async Task<Result<WeeklyResultResponseDto>> UpdateAsync(int id, WeeklyResultUpdateDto dto, CancellationToken ct = default)
    {
        var weekResult = await uow.WeekResults.GetByIdAsync(id, ct);
        if (weekResult is null)
            return Result<WeeklyResultResponseDto>.Fail("Week result not found", ErrorType.NotFound);

        if (dto.BonusScore.HasValue) weekResult.BonusScore = dto.BonusScore.Value;
        if (dto.ExamScore.HasValue) weekResult.ExamScore = dto.ExamScore.Value;
        if (dto.MentorComment is not null) weekResult.MentorComment = dto.MentorComment;

        weekResult.TotalScore = weekResult.AttendanceScore + weekResult.BonusScore + weekResult.ExamScore;
        weekResult.UpdatedAt = DateTime.UtcNow;

        uow.WeekResults.Update(weekResult);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.WeekResults.GetByIdAsync(id, ct);

        return Result<WeeklyResultResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var weekResult = await uow.WeekResults.GetByIdAsync(id, ct);
        if (weekResult is null)
            return Result<bool>.Fail("Week result not found", ErrorType.NotFound);

        uow.WeekResults.Delete(weekResult);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<WeekSummaryDto>> GetByGroupAndWeekAsync(int groupId, int weekNumber, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(groupId, ct);
        if (group is null)
            return Result<WeekSummaryDto>.Fail("Group not found", ErrorType.NotFound);

        var results = await uow.WeekResults.GetByGroupAndWeekAsync(groupId, weekNumber, ct);
        var resultList = results.ToList();

        var topStudent = resultList.MaxBy(wr => wr.TotalScore);
        var averageScore = resultList.Any() ? resultList.Average(wr => wr.TotalScore) : 0;

        var dto = new WeekSummaryDto
        {
            WeekNumber = weekNumber,
            GroupId = groupId,
            GroupName = group.Name,
            Results = resultList.Select(MapToResponseDto),
            GroupAverageScore = Math.Round(averageScore, 2),
            TopStudentName = topStudent is not null
                ? $"{topStudent.Student.User.FirstName} {topStudent.Student.User.LastName}"
                : string.Empty
        };

        return Result<WeekSummaryDto>.Ok(dto);
    }

    public async Task<Result<IEnumerable<WeeklyResultResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(groupId, ct);
        if (group is null)
            return Result<IEnumerable<WeeklyResultResponseDto>>.Fail("Group not found", ErrorType.NotFound);

        var results = await uow.WeekResults.GetByGroupIdAsync(groupId, ct);
        var result = results.Select(MapToResponseDto);

        return Result<IEnumerable<WeeklyResultResponseDto>>.Ok(result);
    }

    public async Task<Result<bool>> RecalculateTotalAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByIdAsync(studentId, ct);
        if (student is null)
            return Result<bool>.Fail("Student not found", ErrorType.NotFound);

        var group = await uow.Groups.GetByIdAsync(groupId, ct);
        if (group is null)
            return Result<bool>.Fail("Group not found", ErrorType.NotFound);

        var weekResult = await uow.WeekResults.GetByStudentGroupWeekAsync(studentId, groupId, weekNumber, ct);
        if (weekResult is null)
            return Result<bool>.Fail("Week result not found", ErrorType.NotFound);

        await uow.WeekResults.RecalculateTotalAsync(studentId, groupId, weekNumber, ct);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<IEnumerable<WeeklyResultResponseDto>>> GetMyResultsAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<WeeklyResultResponseDto>>.Fail("Student not found", ErrorType.NotFound);

        var isInGroup = await uow.GroupStudents.IsStudentInGroupAsync(groupId, student.Id, ct);
        if (!isInGroup)
            return Result<IEnumerable<WeeklyResultResponseDto>>.Fail("Student is not in this group", ErrorType.Validation);

        var results = await uow.WeekResults.GetByStudentIdAsync(student.Id, ct);
        var filtered = results.Where(wr => wr.GroupId == groupId).Select(MapToResponseDto);

        return Result<IEnumerable<WeeklyResultResponseDto>>.Ok(filtered);
    }

    public async Task<Result<WeeklyResultResponseDto>> GetMyBestWeekAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<WeeklyResultResponseDto>.Fail("Student not found", ErrorType.NotFound);

        var bestWeek = await uow.WeekResults.GetBestWeekAsync(student.Id, groupId, ct);
        if (bestWeek is null)
            return Result<WeeklyResultResponseDto>.Fail("No results found", ErrorType.NotFound);

        return Result<WeeklyResultResponseDto>.Ok(MapToResponseDto(bestWeek));
    }

    public async Task<Result<double>> GetMyAverageScoreAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<double>.Fail("Student not found", ErrorType.NotFound);

        var average = await uow.WeekResults.GetAverageScoreAsync(student.Id, groupId, ct);

        return Result<double>.Ok(Math.Round(average, 2));
    }

    private static WeeklyResultResponseDto MapToResponseDto(WeekResult wr) => new()
    {
        Id = wr.Id,
        StudentId = wr.StudentId,
        StudentName = $"{wr.Student.User.FirstName} {wr.Student.User.LastName}",
        GroupId = wr.GroupId,
        GroupName = wr.Group.Name,
        WeekNumber = wr.WeekNumber,
        AttendanceScore = wr.AttendanceScore,
        BonusScore = wr.BonusScore,
        ExamScore = wr.ExamScore,
        TotalScore = wr.TotalScore,
        MentorComment = wr.MentorComment,
        CreatedAt = wr.CreatedAt
    };
}