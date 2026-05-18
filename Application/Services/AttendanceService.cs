using Application.Dtos.AttendanceDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class AttendanceService(IUnitOfWork uow) : IAttendanceService
{
    // ───── Basic CRUD ─────

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
            return Result<AttendanceResponseDto>.Fail("Attendance already exists", ErrorType.Conflict);

        var attendance = new Attendance
        {
            LessonId = dto.LessonId,
            StudentId = dto.StudentId,
            IsPresent = dto.IsPresent,
            AbsenceReason = dto.AbsenceReason,
            MentorNote = dto.MentorNote,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Attendances.AddAsync(attendance, ct);
        await uow.SaveChangesAsync(ct);

        return Result<AttendanceResponseDto>.Ok(MapToResponseDto(attendance));
    }

    public async Task<Result<AttendanceResponseDto>> UpdateAsync(int id, AttendanceUpdateDto dto, CancellationToken ct = default)
    {
        var attendance = await uow.Attendances.GetByIdAsync(id, ct);
        if (attendance is null)
            return Result<AttendanceResponseDto>.Fail("Attendance not found", ErrorType.NotFound);

        attendance.IsPresent = dto.IsPresent;
        attendance.AbsenceReason = dto.AbsenceReason;
        if (dto.MentorNote is not null) attendance.MentorNote = dto.MentorNote;
        attendance.UpdatedAt = DateTime.UtcNow;

        uow.Attendances.Update(attendance);
        await uow.SaveChangesAsync(ct);

        return Result<AttendanceResponseDto>.Ok(MapToResponseDto(attendance));
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

    public async Task<Result<AttendanceResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var attendance = await uow.Attendances.GetByIdAsync(id, ct);
        if (attendance is null)
            return Result<AttendanceResponseDto>.Fail("Attendance not found", ErrorType.NotFound);

        return Result<AttendanceResponseDto>.Ok(MapToResponseDto(attendance));
    }

    // ───── Mentor: Lesson ─────

    public async Task<Result<IEnumerable<AttendanceResponseDto>>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default)
    {
        var attendances = await uow.Attendances.GetByLessonIdAsync(lessonId, ct);
        return Result<IEnumerable<AttendanceResponseDto>>.Ok(attendances.Select(MapToResponseDto));
    }

    public async Task<Result<bool>> MarkAllByLessonAsync(int lessonId, bool isPresent, CancellationToken ct = default)
    {
        var attendances = await uow.Attendances.GetByLessonIdAsync(lessonId, ct);
        foreach (var a in attendances)
        {
            a.IsPresent = isPresent;
            if (!isPresent) a.AbsenceReason = null;
            a.UpdatedAt = DateTime.UtcNow;
            uow.Attendances.Update(a);
        }
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<int>> GetPresentCountByLessonAsync(int lessonId, CancellationToken ct = default)
    {
        var attendances = await uow.Attendances.GetByLessonIdAsync(lessonId, ct);
        return Result<int>.Ok(attendances.Count(a => a.IsPresent));
    }

    public async Task<Result<int>> GetAbsentCountByLessonAsync(int lessonId, CancellationToken ct = default)
    {
        var attendances = await uow.Attendances.GetByLessonIdAsync(lessonId, ct);
        return Result<int>.Ok(attendances.Count(a => !a.IsPresent));
    }

    // ───── Mentor: Week ─────

    public async Task<Result<IEnumerable<AttendanceResponseDto>>> GetByWeekAsync(int groupId, int weekNumber, CancellationToken ct = default)
    {
        var attendances = await uow.Attendances.GetByWeekAsync(groupId, weekNumber, ct);
        return Result<IEnumerable<AttendanceResponseDto>>.Ok(attendances.Select(MapToResponseDto));
    }

    public async Task<Result<int>> GetPresentCountByWeekAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default)
    {
        var count = await uow.Attendances.GetPresentCountByWeekAsync(studentId, groupId, weekNumber, ct);
        return Result<int>.Ok(count);
    }

    // ───── Mentor: Group ─────

    public async Task<Result<IEnumerable<AttendanceResponseDto>>> GetByGroupAsync(int groupId, CancellationToken ct = default)
    {
        var lessons = await uow.Lessons.GetByGroupIdAsync(groupId, ct);
        var lessonIds = lessons.Select(l => l.Id);
        var attendances = new List<Attendance>();
        foreach (var id in lessonIds)
        {
            var list = await uow.Attendances.GetByLessonIdAsync(id, ct);
            attendances.AddRange(list);
        }
        return Result<IEnumerable<AttendanceResponseDto>>.Ok(attendances.Select(MapToResponseDto));
    }

    public async Task<Result<int>> GetAbsenceCountByGroupAsync(int studentId, int groupId, CancellationToken ct = default)
    {
        var count = await uow.Attendances.GetAbsenceCountAsync(studentId, groupId, ct);
        return Result<int>.Ok(count);
    }

    public async Task<Result<double>> GetAttendanceRateByGroupAsync(int studentId, int groupId, CancellationToken ct = default)
    {
        var rate = await uow.Attendances.GetAttendanceRateAsync(studentId, groupId, ct);
        return Result<double>.Ok(rate);
    }

    // ───── Mentor: Bulk ─────

    public async Task<Result<bool>> BulkCreateAsync(IEnumerable<AttendanceCreateDto> dtos, CancellationToken ct = default)
    {
        foreach (var dto in dtos)
        {
            var attendance = new Attendance
            {
                LessonId = dto.LessonId,
                StudentId = dto.StudentId,
                IsPresent = dto.IsPresent,
                AbsenceReason = dto.AbsenceReason,
                MentorNote = dto.MentorNote,
                CreatedAt = DateTime.UtcNow
            };
            await uow.Attendances.AddAsync(attendance, ct);
        }
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> BulkUpdateAsync(IEnumerable<AttendanceUpdateBulkDto> dtos, CancellationToken ct = default)
    {
        foreach (var dto in dtos)
        {
            var attendance = await uow.Attendances.GetByIdAsync(dto.AttendanceId, ct);
            if (attendance is null) continue;

            attendance.IsPresent = dto.IsPresent;
            if (dto.MentorNote is not null) attendance.MentorNote = dto.MentorNote;
            attendance.UpdatedAt = DateTime.UtcNow;
            uow.Attendances.Update(attendance);
        }
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    // ───── Student ─────

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
            return Result<bool>.Fail("Student was present", ErrorType.Validation);

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

        var attendances = await uow.Attendances.GetByStudentAndGroupAsync(student.Id, groupId, ct);
        return Result<IEnumerable<AttendanceResponseDto>>.Ok(attendances.Select(MapToResponseDto));
    }

    public async Task<Result<double>> GetMyAttendanceRateAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<double>.Fail("Student not found", ErrorType.NotFound);

        var rate = await uow.Attendances.GetAttendanceRateAsync(student.Id, groupId, ct);
        return Result<double>.Ok(Math.Round(rate, 2));
    }

    public async Task<Result<int>> GetMyAbsenceCountAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<int>.Fail("Student not found", ErrorType.NotFound);

        var count = await uow.Attendances.GetAbsenceCountAsync(student.Id, groupId, ct);
        return Result<int>.Ok(count);
    }

    public async Task<Result<double>> GetMyAverageScoreAsync(int userId, int groupId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<double>.Fail("Student not found", ErrorType.NotFound);

        var rate = await uow.Attendances.GetAttendanceRateAsync(student.Id, groupId, ct);
        return Result<double>.Ok(Math.Round(rate, 2));
    }

    // ───── Mapper ─────

    private static AttendanceResponseDto MapToResponseDto(Attendance a) => new()
    {
        Id = a.Id,
        LessonId = a.LessonId,
        LessonDate = a.Lesson?.LessonDate ?? DateTime.MinValue,
        WeekNumber = a.Lesson?.WeekNumber ?? 0,
        StudentId = a.StudentId,
        StudentName = a.Student?.User != null
            ? $"{a.Student.User.FirstName} {a.Student.User.LastName}"
            : $"Student {a.StudentId}",
        IsPresent = a.IsPresent,
        AbsenceReason = a.AbsenceReason,
        MentorNote = a.MentorNote,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
}