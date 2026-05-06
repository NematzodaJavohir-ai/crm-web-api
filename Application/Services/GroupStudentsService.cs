using Application.Dtos.GroupStudentDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Enums;

namespace Application.Services;

public class GroupStudentsService(IUnitOfWork uow) : IGroupStudentService
{
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

        var groupStudent = new Domain.Entities.GroupStudent
        {
            GroupId = dto.GroupId,
            StudentId = dto.StudentId,
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        };

        await uow.GroupStudents.AddAsync(groupStudent, ct);
        await uow.SaveChangesAsync(ct);

        var saved = await uow.GroupStudents.GetByGroupAndStudentAsync(dto.GroupId, dto.StudentId, ct);

        return Result<GroupStudentResponseDto>.Ok(MapToResponseDto(saved!));
    }

    public async Task<Result<bool>> RemoveStudentAsync(RemoveStudentFromGroupDto dto, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(dto.GroupId, ct);
        if (group is null)
            return Result<bool>.Fail("Group not found", ErrorType.NotFound);

        var student = await uow.Students.GetByIdAsync(dto.StudentId, ct);
        if (student is null)
            return Result<bool>.Fail("Student not found", ErrorType.NotFound);

        var groupStudent = await uow.GroupStudents.GetByGroupAndStudentAsync(dto.GroupId, dto.StudentId, ct);
        if (groupStudent is null)
            return Result<bool>.Fail("Student is not in this group", ErrorType.NotFound);

        if (!groupStudent.IsActive)
            return Result<bool>.Fail("Student already removed from this group", ErrorType.Validation);

        await uow.GroupStudents.RemoveStudentAsync(dto.GroupId, dto.StudentId, dto.Reason, ct);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<IEnumerable<GroupStudentResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(groupId, ct);
        if (group is null)
            return Result<IEnumerable<GroupStudentResponseDto>>.Fail("Group not found", ErrorType.NotFound);

        var groupStudents = await uow.GroupStudents.GetByGroupIdAsync(groupId, ct);
        var result = groupStudents.Select(MapToResponseDto);

        return Result<IEnumerable<GroupStudentResponseDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<GroupStudentResponseDto>>> GetActiveByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(groupId, ct);
        if (group is null)
            return Result<IEnumerable<GroupStudentResponseDto>>.Fail("Group not found", ErrorType.NotFound);

        var groupStudents = await uow.GroupStudents.GetActiveByGroupIdAsync(groupId, ct);
        var result = groupStudents.Select(MapToResponseDto);

        return Result<IEnumerable<GroupStudentResponseDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<GroupStudentResponseDto>>> GetMyGroupsAsync(int userId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<GroupStudentResponseDto>>.Fail("Student not found", ErrorType.NotFound);

        var groupStudents = await uow.GroupStudents.GetByStudentIdAsync(student.Id, ct);
        var result = groupStudents.Select(MapToResponseDto);

        return Result<IEnumerable<GroupStudentResponseDto>>.Ok(result);
    }

    private static GroupStudentResponseDto MapToResponseDto(Domain.Entities.GroupStudent gs) => new()
    {
        Id = gs.Id,
        GroupId = gs.GroupId,
        GroupName = gs.Group.Name,
        StudentId = gs.StudentId,
        StudentName = $"{gs.Student.User.FirstName} {gs.Student.User.LastName}",
        StudentEmail = gs.Student.User.Email,
        JoinedAt = gs.JoinedAt,
        LeftAt = gs.LeftAt,
        IsActive = gs.IsActive,
        RemoveReason = gs.RemoveReason
    };
}