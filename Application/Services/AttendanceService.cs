using Application.Dtos.AttendanceDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class AttendanceService(IUnitOfWork uow) : IAttendanceService
{
    public async Task<Result<AttendanceResponseDto>> CreateAsync(AttendanceCreateDto dto, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetByIdAsync(dto.LessonId, ct);
        if (lesson is null)
            return Result<AttendanceResponseDto>.Fail("Lesson not found", ErrorType.NotFound);

        var student = await uow.Students.GetByIdAsync(dto.StudentId, ct);
        if (student is null)
            return Result<AttendanceResponseDto>.Fail("Student not found", ErrorType.NotFound);

        var isInGroup = await uow.GroupStudents.IsStudentInGroupAsync(lesson.GroupId, dto.StudentId, ct);
        if (!isInGroup)
            return Result<AttendanceResponseDto>.Fail("Student is not in this group", ErrorType.Validation);

        var alreadyExists = await uow.Attendances.AlreadyExistsAsync(dto.LessonId, dto.StudentId, ct);
        if (alreadyExists)
            return Result<AttendanceResponseDto>.Fail("Attendance already exists for this student", ErrorType.Conflict);

        if (!dto.IsPresent && dto.Score > 0)
            return Result<AttendanceResponseDto>.Fail("Cannot give score to absent student", ErrorType.Validation);

        if (!dto.HomeworkDone && dto.HomeworkScore > 0)
            return Result<AttendanceResponseDto>.Fail("Cannot give homework score if homework is not done", ErrorType.Validation);

        var attendance = new Attendance
        {
            LessonId = dto.LessonId,
            StudentId = dto.StudentId,
            IsPresent = dto.IsPresent,
            Score = dto.Score,
            MentorNote = dto.MentorNote,
            HomeworkDone = dto.HomeworkDone,
            HomeworkScore = dto.HomeworkScore,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Attendances.AddAsync(attendance, ct);
        await uow.SaveChangesAsync(ct);

        
        var saved = await uow.Attendances.GetByLessonAndStudentAsync(dto.LessonId, dto.StudentId, ct);

        return Result<AttendanceResponseDto>.Ok(MapToResponseDto(saved!));
    }

    public async Task<Result<AttendanceResponseDto>> UpdateAsync(int id, AttendanceUpdateDto dto, CancellationToken ct = default)
    {
        var attendance = await uow.Attendances.GetByIdAsync(id, ct);
        if (attendance is null)
            return Result<AttendanceResponseDto>.Fail("Attendance not found", ErrorType.NotFound);

        if (dto.IsPresent.HasValue)
        {
            if (!dto.IsPresent.Value)
            {
                attendance.Score = 0;
                attendance.IsPresent = false;
            }
            else
            {
                attendance.IsPresent = true;
            }
        }

        if (dto.Score.HasValue)
        {
            if (!attendance.IsPresent && dto.Score.Value > 0)
                return Result<AttendanceResponseDto>.Fail("Cannot give score to absent student", ErrorType.Validation);

            attendance.Score = dto.Score.Value;
        }

        if (dto.HomeworkDone.HasValue)
        {
            if (!dto.HomeworkDone.Value)
                attendance.HomeworkScore = 0;

            attendance.HomeworkDone = dto.HomeworkDone.Value;
        }

        if (dto.HomeworkScore.HasValue)
        {
            if (!attendance.HomeworkDone && dto.HomeworkScore.Value > 0)
                return Result<AttendanceResponseDto>.Fail("Cannot give homework score if homework is not done", ErrorType.Validation);

            attendance.HomeworkScore = dto.HomeworkScore.Value;
        }

        if (dto.MentorNote is not null) attendance.MentorNote = dto.MentorNote;

        attendance.UpdatedAt = DateTime.UtcNow;

        uow.Attendances.Update(attendance);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Attendances.GetByLessonAndStudentAsync(attendance.LessonId, attendance.StudentId, ct);

        return Result<AttendanceResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var attendance = await uow.Attendances.GetByIdAsync(id, ct);
        if (attendance is null)
            return Result<bool>.Fail("Attendance not found", ErrorType.NotFound);

        uow.Attendances.Delete(attendance);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<IEnumerable<AttendanceResponseDto>>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default)
    {
        var lesson = await uow.Lessons.GetByIdAsync(lessonId, ct);
        if (lesson is null)
            return Result<IEnumerable<AttendanceResponseDto>>.Fail("Lesson not found", ErrorType.NotFound);

        var attendances = await uow.Attendances.GetByLessonIdAsync(lessonId, ct);
        var result = attendances.Select(MapToResponseDto);

        return Result<IEnumerable<AttendanceResponseDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<AttendanceResponseDto>>> GetByWeekAsync(int groupId, int weekNumber, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(groupId, ct);
        if (group is null)
            return Result<IEnumerable<AttendanceResponseDto>>.Fail("Group not found", ErrorType.NotFound);

        var attendances = await uow.Attendances.GetByWeekAsync(groupId, weekNumber, ct);
        var result = attendances.Select(MapToResponseDto);

        return Result<IEnumerable<AttendanceResponseDto>>.Ok(result);
    }

    public async Task<Result<bool>> AddAbsenceReasonAsync(int attendanceId, int userId, AddAbsenceReasonDto dto, CancellationToken ct = default)
    {
        var attendance = await uow.Attendances.GetByIdAsync(attendanceId, ct);
        if (attendance is null)
            return Result<bool>.Fail("Attendance not found", ErrorType.NotFound);

        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<bool>.Fail("Student not found", ErrorType.NotFound);

        if (attendance.StudentId != student.Id)
            return Result<bool>.Fail("Forbidden", ErrorType.Forbidden);

        if (attendance.IsPresent)
            return Result<bool>.Fail("Student was present, no need for absence reason", ErrorType.Validation);

        if (attendance.AbsenceReason is not null)
            return Result<bool>.Fail("Absence reason already added", ErrorType.Conflict);

        attendance.AbsenceReason = dto.AbsenceReason;
        attendance.UpdatedAt = DateTime.UtcNow;

        uow.Attendances.Update(attendance);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<IEnumerable<AttendanceResponseDto>>> GetMyAttendancesAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<AttendanceResponseDto>>.Fail("Student not found", ErrorType.NotFound);

        var isInGroup = await uow.GroupStudents.IsStudentInGroupAsync(groupId, student.Id, ct);
        if (!isInGroup)
            return Result<IEnumerable<AttendanceResponseDto>>.Fail("Student is not in this group", ErrorType.Validation);

        var attendances = await uow.Attendances.GetByStudentAndGroupAsync(student.Id, groupId, ct);
        var result = attendances.Select(MapToResponseDto);

        return Result<IEnumerable<AttendanceResponseDto>>.Ok(result);
    }

    public async Task<Result<double>> GetMyAverageScoreAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<double>.Fail("Student not found", ErrorType.NotFound);

        var average = await uow.Attendances.GetAverageScoreAsync(student.Id, groupId, ct);

        return Result<double>.Ok(Math.Round(average, 2));
    }

    public async Task<Result<int>> GetMyAbsenceCountAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<int>.Fail("Student not found", ErrorType.NotFound);

        var count = await uow.Attendances.GetAbsenceCountAsync(student.Id, groupId, ct);

        return Result<int>.Ok(count);
    }

    private static AttendanceResponseDto MapToResponseDto(Attendance a) => new()
    {
        Id = a.Id,
        LessonId = a.LessonId,
        LessonDate = a.Lesson.LessonDate,
        WeekNumber = a.Lesson.WeekNumber,
        StudentId = a.StudentId,
        StudentName = $"{a.Student.User.FirstName} {a.Student.User.LastName}",
        IsPresent = a.IsPresent,
        Score = a.Score,
        AbsenceReason = a.AbsenceReason,
        MentorNote = a.MentorNote,
        HomeworkDone = a.HomeworkDone,
        HomeworkScore = a.HomeworkScore,
        CreatedAt = a.CreatedAt
    };
}