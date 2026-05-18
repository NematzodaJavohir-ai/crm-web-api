using Application.Dtos.GroupStudentDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class GroupStudentService(IUnitOfWork uow) : IGroupStudentService
{
    // ───── Basic Operations ─────

    public async Task<Result<GroupStudentResponseDto>> AddStudentAsync(AddStudentToGroupDto dto, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(dto.GroupId, ct);
        if (group is null)
            return Result<GroupStudentResponseDto>.Fail("Group not found", ErrorType.NotFound);

        var student = await uow.Students.GetByIdAsync(dto.StudentId, ct);
        if (student is null)
            return Result<GroupStudentResponseDto>.Fail("Student not found", ErrorType.NotFound);

        var alreadyExists = await uow.GroupStudents.IsStudentInGroupAsync(dto.GroupId, dto.StudentId, ct);
        if (alreadyExists)
            return Result<GroupStudentResponseDto>.Fail("Student already in this group", ErrorType.Conflict);

        var studentCount = await uow.GroupStudents.GetActiveStudentCountAsync(dto.GroupId, ct);
        if (studentCount >= group.MaxStudents)
            return Result<GroupStudentResponseDto>.Fail($"Group is full. Max students: {group.MaxStudents}", ErrorType.Conflict);

        if (group.Status != GroupStatus.Active)
            return Result<GroupStudentResponseDto>.Fail("Cannot add student to inactive group", ErrorType.Validation);

        var groupStudent = new GroupStudent
        {
            GroupId = dto.GroupId,
            StudentId = dto.StudentId,
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        };

        await uow.GroupStudents.AddAsync(groupStudent, ct);
        await uow.SaveChangesAsync(ct);

        var saved = await uow.GroupStudents.GetActiveByStudentAndGroupAsync(dto.StudentId, dto.GroupId, ct);
        return Result<GroupStudentResponseDto>.Ok(MapToResponseDto(saved!));
    }

    public async Task<Result<bool>> RemoveStudentAsync(RemoveStudentFromGroupDto dto, CancellationToken ct = default)
    {
        var groupStudent = await uow.GroupStudents.GetActiveByStudentAndGroupAsync(dto.StudentId, dto.GroupId, ct);
        if (groupStudent is null)
            return Result<bool>.Fail("Student is not in this group", ErrorType.NotFound);

        groupStudent.IsActive = false;
        groupStudent.LeftAt = DateTime.UtcNow;
        groupStudent.RemoveReason = dto.Reason;

        uow.GroupStudents.Update(groupStudent);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<GroupStudentResponseDto>> RestoreStudentAsync(int groupId, int studentId, CancellationToken ct = default)
    {
        var groupStudent = await uow.GroupStudents.GetByStudentIdAsync(studentId, ct);
        var record = groupStudent.FirstOrDefault(gs => gs.GroupId == groupId && !gs.IsActive);
        if (record is null)
            return Result<GroupStudentResponseDto>.Fail("Record not found", ErrorType.NotFound);

        var group = await uow.Groups.GetByIdAsync(groupId, ct);
        if (group!.Status != GroupStatus.Active)
            return Result<GroupStudentResponseDto>.Fail("Cannot restore to inactive group", ErrorType.Validation);

        var studentCount = await uow.GroupStudents.GetActiveStudentCountAsync(groupId, ct);
        if (studentCount >= group.MaxStudents)
            return Result<GroupStudentResponseDto>.Fail($"Group is full. Max: {group.MaxStudents}", ErrorType.Conflict);

        record.IsActive = true;
        record.LeftAt = null;
        record.RemoveReason = null;
        record.JoinedAt = DateTime.UtcNow;

        uow.GroupStudents.Update(record);
        await uow.SaveChangesAsync(ct);

        return Result<GroupStudentResponseDto>.Ok(MapToResponseDto(record));
    }

    public async Task<Result<TransferResultDto>> TransferStudentAsync(TransferStudentDto dto, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByIdAsync(dto.StudentId, ct);
        if (student is null)
            return Result<TransferResultDto>.Fail("Student not found", ErrorType.NotFound);

        var fromGroup = await uow.Groups.GetByIdAsync(dto.FromGroupId, ct);
        if (fromGroup is null)
            return Result<TransferResultDto>.Fail("Source group not found", ErrorType.NotFound);

        var toGroup = await uow.Groups.GetByIdAsync(dto.ToGroupId, ct);
        if (toGroup is null)
            return Result<TransferResultDto>.Fail("Target group not found", ErrorType.NotFound);

        if (dto.FromGroupId == dto.ToGroupId)
            return Result<TransferResultDto>.Fail("Source and target groups cannot be the same", ErrorType.Validation);

        var isInFromGroup = await uow.GroupStudents.IsStudentInGroupAsync(dto.StudentId, dto.FromGroupId, ct);
        if (!isInFromGroup)
            return Result<TransferResultDto>.Fail("Student is not in source group", ErrorType.Validation);

        var isAlreadyInToGroup = await uow.GroupStudents.IsStudentInGroupAsync(dto.StudentId, dto.ToGroupId, ct);
        if (isAlreadyInToGroup)
            return Result<TransferResultDto>.Fail("Student is already in target group", ErrorType.Conflict);

        var studentCount = await uow.GroupStudents.GetActiveStudentCountAsync(dto.ToGroupId, ct);
        if (studentCount >= toGroup.MaxStudents)
            return Result<TransferResultDto>.Fail($"Target group is full. Max: {toGroup.MaxStudents}", ErrorType.Conflict);

        if (toGroup.Status != GroupStatus.Active)
            return Result<TransferResultDto>.Fail("Target group is not active", ErrorType.Validation);

        var fromRecord = await uow.GroupStudents.GetActiveByStudentAndGroupAsync(dto.StudentId, dto.FromGroupId, ct);
        fromRecord!.IsActive = false;
        fromRecord.LeftAt = DateTime.UtcNow;
        fromRecord.RemoveReason = dto.Reason ?? "Transferred";
        uow.GroupStudents.Update(fromRecord);

        var newRecord = new GroupStudent
        {
            GroupId = dto.ToGroupId,
            StudentId = dto.StudentId,
            JoinedAt = DateTime.UtcNow,
            IsActive = true,
            TransferredFromGroupStudentId = fromRecord.Id
        };
        await uow.GroupStudents.AddAsync(newRecord, ct);

        fromRecord.TransferredToGroupStudentId = newRecord.Id;
        uow.GroupStudents.Update(fromRecord);

        await uow.SaveChangesAsync(ct);

        return Result<TransferResultDto>.Ok(new TransferResultDto
        {
            Success = true,
            FromGroup = MapToResponseDto(fromRecord),
            ToGroup = MapToResponseDto(newRecord),
            Message = "Student transferred successfully"
        });
    }

    // ───── Bulk Operations ─────

    public async Task<Result<BulkOperationResultDto>> BulkAddStudentsAsync(BulkAddStudentsDto dto, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(dto.GroupId, ct);
        if (group is null)
            return Result<BulkOperationResultDto>.Fail("Group not found", ErrorType.NotFound);

        var errors = new List<BulkOperationErrorDto>();
        var succeeded = 0;

        foreach (var studentId in dto.StudentIds)
        {
            var student = await uow.Students.GetByIdAsync(studentId, ct);
            if (student is null)
            {
                errors.Add(new BulkOperationErrorDto { StudentId = studentId, Reason = "Student not found" });
                continue;
            }

            var alreadyExists = await uow.GroupStudents.IsStudentInGroupAsync(dto.GroupId, studentId, ct);
            if (alreadyExists)
            {
                errors.Add(new BulkOperationErrorDto { StudentId = studentId, StudentName = $"{student.User.FirstName} {student.User.LastName}", Reason = "Already in group" });
                continue;
            }

            await uow.GroupStudents.AddAsync(new GroupStudent
            {
                GroupId = dto.GroupId,
                StudentId = studentId,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            }, ct);
            succeeded++;
        }

        await uow.SaveChangesAsync(ct);

        return Result<BulkOperationResultDto>.Ok(new BulkOperationResultDto
        {
            TotalRequested = dto.StudentIds.Count(),
            Succeeded = succeeded,
            Failed = errors.Count,
            Errors = errors
        });
    }

    public async Task<Result<BulkOperationResultDto>> BulkRemoveStudentsAsync(BulkRemoveStudentsDto dto, CancellationToken ct = default)
    {
        var errors = new List<BulkOperationErrorDto>();
        var succeeded = 0;

        foreach (var studentId in dto.StudentIds)
        {
            var record = await uow.GroupStudents.GetActiveByStudentAndGroupAsync(studentId, dto.GroupId, ct);
            if (record is null)
            {
                errors.Add(new BulkOperationErrorDto { StudentId = studentId, Reason = "Not in group" });
                continue;
            }

            record.IsActive = false;
            record.LeftAt = DateTime.UtcNow;
            record.RemoveReason = dto.Reason;
            uow.GroupStudents.Update(record);
            succeeded++;
        }

        await uow.SaveChangesAsync(ct);

        return Result<BulkOperationResultDto>.Ok(new BulkOperationResultDto
        {
            TotalRequested = dto.StudentIds.Count(),
            Succeeded = succeeded,
            Failed = errors.Count,
            Errors = errors
        });
    }

    // ───── Queries: Single ─────

    public async Task<Result<GroupStudentResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var gs = await uow.GroupStudents.GetByIdAsync(id, ct);
        if (gs is null)
            return Result<GroupStudentResponseDto>.Fail("Not found", ErrorType.NotFound);

        return Result<GroupStudentResponseDto>.Ok(MapToResponseDto(gs));
    }

    public async Task<Result<GroupStudentResponseDto>> GetActiveByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default)
    {
        var gs = await uow.GroupStudents.GetActiveByStudentAndGroupAsync(studentId, groupId, ct);
        if (gs is null)
            return Result<GroupStudentResponseDto>.Fail("Student not active in this group", ErrorType.NotFound);

        return Result<GroupStudentResponseDto>.Ok(MapToResponseDto(gs));
    }

    // ───── Queries: Lists ─────

    public async Task<Result<IEnumerable<GroupStudentResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var list = await uow.GroupStudents.GetByGroupIdAsync(groupId, ct);
        return Result<IEnumerable<GroupStudentResponseDto>>.Ok(list.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<GroupStudentResponseDto>>> GetActiveByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var list = await uow.GroupStudents.GetActiveByGroupIdAsync(groupId, ct);
        return Result<IEnumerable<GroupStudentResponseDto>>.Ok(list.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<GroupStudentResponseDto>>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
    {
        var list = await uow.GroupStudents.GetByStudentIdAsync(studentId, ct);
        return Result<IEnumerable<GroupStudentResponseDto>>.Ok(list.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<GroupStudentResponseDto>>> GetByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default)
    {
        var list = await uow.GroupStudents.GetByStudentIdAsync(studentId, ct);
        var filtered = list.Where(gs => gs.GroupId == groupId);
        return Result<IEnumerable<GroupStudentResponseDto>>.Ok(filtered.Select(MapToResponseDto));
    }

    // ───── History ─────

    public async Task<Result<IEnumerable<GroupStudentHistoryDto>>> GetStudentHistoryAsync(int studentId, CancellationToken ct = default)
    {
        var list = await uow.GroupStudents.GetByStudentIdAsync(studentId, ct);
        return Result<IEnumerable<GroupStudentHistoryDto>>.Ok(list.Select(MapToHistoryDto));
    }

    public async Task<Result<IEnumerable<GroupStudentHistoryDto>>> GetGroupHistoryAsync(int groupId, CancellationToken ct = default)
    {
        var list = await uow.GroupStudents.GetByGroupIdAsync(groupId, ct);
        return Result<IEnumerable<GroupStudentHistoryDto>>.Ok(list.Select(MapToHistoryDto));
    }

    public async Task<Result<IEnumerable<TransferHistoryDto>>> GetTransferHistoryAsync(int studentId, CancellationToken ct = default)
    {
        var list = await uow.GroupStudents.GetTransferHistoryAsync(studentId, ct);
        return Result<IEnumerable<TransferHistoryDto>>.Ok(list.Select(gs => new TransferHistoryDto
        {
            Id = gs.Id,
            StudentId = gs.StudentId,
            StudentName = $"{gs.Student?.User.FirstName} {gs.Student?.User.LastName}",
            FromGroupId = gs.TransferredFrom?.GroupId ?? 0,
            FromGroupName = gs.TransferredFrom?.Group?.Name,
            ToGroupId = gs.GroupId,
            ToGroupName = gs.Group?.Name,
            TransferDate = gs.JoinedAt,
            Reason = gs.RemoveReason
        }));
    }

    public async Task<Result<IEnumerable<TransferHistoryDto>>> GetTransferHistoryByGroupAsync(int groupId, CancellationToken ct = default)
    {
        var list = await uow.GroupStudents.GetByGroupIdAsync(groupId, ct);
        var transfers = list.Where(gs => gs.TransferredFromGroupStudentId != null || gs.TransferredToGroupStudentId != null);
        return Result<IEnumerable<TransferHistoryDto>>.Ok(transfers.Select(gs => new TransferHistoryDto
        {
            Id = gs.Id,
            StudentId = gs.StudentId,
            StudentName = $"{gs.Student?.User.FirstName} {gs.Student?.User.LastName}",
            FromGroupId = gs.TransferredFrom?.GroupId ?? gs.GroupId,
            FromGroupName = gs.TransferredFrom?.Group?.Name ?? gs.Group?.Name,
            ToGroupId = gs.TransferredTo?.GroupId ?? gs.GroupId,
            ToGroupName = gs.TransferredTo?.Group?.Name ?? gs.Group?.Name,
            TransferDate = gs.JoinedAt,
            Reason = gs.RemoveReason
        }));
    }

    // ───── Statistics ─────

    public async Task<Result<GroupStudentStatsDto>> GetGroupStatsAsync(int groupId, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(groupId, ct);
        if (group is null)
            return Result<GroupStudentStatsDto>.Fail("Group not found", ErrorType.NotFound);

        var all = await uow.GroupStudents.GetByGroupIdAsync(groupId, ct);
        var allList = all.ToList();

        return Result<GroupStudentStatsDto>.Ok(new GroupStudentStatsDto
        {
            GroupId = groupId,
            GroupName = group.Name,
            MaxStudents = group.MaxStudents,
            ActiveStudents = allList.Count(gs => gs.IsActive),
            TotalJoined = allList.Count,
            TotalLeft = allList.Count(gs => !gs.IsActive),
            TransferredIn = allList.Count(gs => gs.TransferredFromGroupStudentId != null),
            TransferredOut = allList.Count(gs => gs.TransferredToGroupStudentId != null),
            RetentionRate = allList.Count > 0 ? (double)allList.Count(gs => gs.IsActive) / allList.Count * 100 : 0,
            AvailableSeats = group.MaxStudents - allList.Count(gs => gs.IsActive)
        });
    }

    public async Task<Result<int>> GetActiveStudentCountAsync(int groupId, CancellationToken ct = default)
    {
        var count = await uow.GroupStudents.GetActiveStudentCountAsync(groupId, ct);
        return Result<int>.Ok(count);
    }

    public async Task<Result<int>> GetTotalJoinedCountAsync(int groupId, CancellationToken ct = default)
    {
        var list = await uow.GroupStudents.GetByGroupIdAsync(groupId, ct);
        return Result<int>.Ok(list.Count());
    }

    public async Task<Result<int>> GetTotalLeftCountAsync(int groupId, CancellationToken ct = default)
    {
        var list = await uow.GroupStudents.GetByGroupIdAsync(groupId, ct);
        return Result<int>.Ok(list.Count(gs => !gs.IsActive));
    }

    public async Task<Result<bool>> IsStudentInGroupAsync(int studentId, int groupId, CancellationToken ct = default)
    {
        var exists = await uow.GroupStudents.IsStudentInGroupAsync(studentId, groupId, ct);
        return Result<bool>.Ok(exists);
    }

    public async Task<Result<bool>> HasAvailableSeatsAsync(int groupId, CancellationToken ct = default)
    {
        var hasSeats = await uow.Groups.HasAvailableSeatsAsync(groupId, ct);
        return Result<bool>.Ok(hasSeats);
    }

    // ───── Student ─────

    public async Task<Result<IEnumerable<GroupStudentResponseDto>>> GetMyGroupsAsync(int userId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<GroupStudentResponseDto>>.Fail("Student not found", ErrorType.NotFound);

        var list = await uow.GroupStudents.GetByStudentIdAsync(student.Id, ct);
        return Result<IEnumerable<GroupStudentResponseDto>>.Ok(list.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<GroupStudentResponseDto>>> GetMyActiveGroupsAsync(int userId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<GroupStudentResponseDto>>.Fail("Student not found", ErrorType.NotFound);

        var list = await uow.GroupStudents.GetByStudentIdAsync(student.Id, ct);
        return Result<IEnumerable<GroupStudentResponseDto>>.Ok(list.Where(gs => gs.IsActive).Select(MapToResponseDto));
    }

    public async Task<Result<int>> GetMyActiveGroupCountAsync(int userId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<int>.Fail("Student not found", ErrorType.NotFound);

        var list = await uow.GroupStudents.GetByStudentIdAsync(student.Id, ct);
        return Result<int>.Ok(list.Count(gs => gs.IsActive));
    }

    // ───── Mappers ─────

    private static GroupStudentResponseDto MapToResponseDto(GroupStudent gs) => new()
    {
        Id = gs.Id,
        GroupId = gs.GroupId,
        GroupName = gs.Group?.Name ?? "",
        CourseName = gs.Group?.Course?.Name ?? "",
        StudentId = gs.StudentId,
        StudentName = gs.Student?.User != null
            ? $"{gs.Student.User.FirstName} {gs.Student.User.LastName}"
            : "",
        StudentEmail = gs.Student?.User?.Email,
        JoinedAt = gs.JoinedAt,
        LeftAt = gs.LeftAt,
        IsActive = gs.IsActive,
        RemoveReason = gs.RemoveReason
    };

    private static GroupStudentHistoryDto MapToHistoryDto(GroupStudent gs) => new()
    {
        Id = gs.Id,
        GroupId = gs.GroupId,
        GroupName = gs.Group?.Name ?? "",
        CourseName = gs.Group?.Course?.Name ?? "",
        MentorName = gs.Group?.Mentor?.User != null
            ? $"{gs.Group.Mentor.User.FirstName} {gs.Group.Mentor.User.LastName}"
            : "",
        StudentName = gs.Student?.User != null
            ? $"{gs.Student.User.FirstName} {gs.Student.User.LastName}"
            : "",
        JoinedAt = gs.JoinedAt,
        LeftAt = gs.LeftAt,
        IsActive = gs.IsActive,
        RemoveReason = gs.RemoveReason,
        IsTransferred = gs.TransferredFromGroupStudentId != null || gs.TransferredToGroupStudentId != null,
        TransferredToGroup = gs.TransferredTo?.Group?.Name,
        TransferredFromGroup = gs.TransferredFrom?.Group?.Name
    };
}