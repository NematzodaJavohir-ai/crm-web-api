using Application.Dtos.SheduleDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class SheduleService(IUnitOfWork uow) : ISheduleService
{
    public async Task<Result<SheduleResponseDto>> CreateAsync(SheduleCreateDto dto, CancellationToken ct = default)
    {
        var group = await uow.Groups.GetByIdAsync(dto.GroupId, ct);
        if (group is null)
            return Result<SheduleResponseDto>.Fail("Group not found", ErrorType.NotFound);

        if (dto.StartTime >= dto.EndTime)
            return Result<SheduleResponseDto>.Fail("StartTime must be before EndTime", ErrorType.Validation);

        var hasConflict = await uow.Shedules.HasConflictAsync(dto.GroupId, dto.Day, dto.StartTime, dto.EndTime, ct);
        if (hasConflict)
            return Result<SheduleResponseDto>.Fail("Schedule conflict on this day and time", ErrorType.Conflict);

        var shedule = new Shedule
        {
            GroupId = dto.GroupId,
            Day = dto.Day,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };

        await uow.Shedules.AddAsync(shedule, ct);
        await uow.SaveChangesAsync(ct);

        var saved = await uow.Shedules.GetByIdAsync(shedule.Id, ct);
        return Result<SheduleResponseDto>.Ok(MapToResponseDto(saved!));
    }

    public async Task<Result<SheduleResponseDto>> UpdateAsync(int id, SheduleUpdateDto dto, CancellationToken ct = default)
    {
        var shedule = await uow.Shedules.GetByIdAsync(id, ct);
        if (shedule is null)
            return Result<SheduleResponseDto>.Fail("Schedule not found", ErrorType.NotFound);

        if (dto.StartTime >= dto.EndTime)
            return Result<SheduleResponseDto>.Fail("StartTime must be before EndTime", ErrorType.Validation);

        shedule.Day = dto.Day;
        shedule.StartTime = dto.StartTime;
        shedule.EndTime = dto.EndTime;

        uow.Shedules.Update(shedule);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Shedules.GetByIdAsync(id, ct);
        return Result<SheduleResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var shedule = await uow.Shedules.GetByIdAsync(id, ct);
        if (shedule is null)
            return Result<bool>.Fail("Schedule not found", ErrorType.NotFound);

        uow.Shedules.Delete(shedule);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<SheduleResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var shedule = await uow.Shedules.GetByIdAsync(id, ct);
        if (shedule is null)
            return Result<SheduleResponseDto>.Fail("Schedule not found", ErrorType.NotFound);

        return Result<SheduleResponseDto>.Ok(MapToResponseDto(shedule));
    }

    public async Task<Result<IEnumerable<SheduleResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var shedules = await uow.Shedules.GetByGroupIdAsync(groupId, ct);
        return Result<IEnumerable<SheduleResponseDto>>.Ok(shedules.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<SheduleResponseDto>>> GetByDayAsync(DayOfWeek day, CancellationToken ct = default)
    {
        var shedules = await uow.Shedules.GetByDayAsync(day, ct);
        return Result<IEnumerable<SheduleResponseDto>>.Ok(shedules.Select(MapToResponseDto));
    }

    public async Task<Result<bool>> HasConflictAsync(int groupId, DayOfWeek day, TimeSpan start, TimeSpan end, CancellationToken ct = default)
    {
        var hasConflict = await uow.Shedules.HasConflictAsync(groupId, day, start, end, ct);
        return Result<bool>.Ok(hasConflict);
    }

    public async Task<Result<IEnumerable<SheduleResponseDto>>> GetMyScheduleAsync(int userId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<SheduleResponseDto>>.Fail("Student not found", ErrorType.NotFound);

        var activeGroups = student.GroupStudents.Where(gs => gs.IsActive).Select(gs => gs.GroupId);
        var shedules = new List<Shedule>();

        foreach (var groupId in activeGroups)
        {
            var groupShedules = await uow.Shedules.GetByGroupIdAsync(groupId, ct);
            shedules.AddRange(groupShedules);
        }

        return Result<IEnumerable<SheduleResponseDto>>.Ok(shedules.Select(MapToResponseDto));
    }

    private static SheduleResponseDto MapToResponseDto(Shedule s) => new()
    {
        Id = s.Id,
        GroupId = s.GroupId,
        GroupName = s.Group?.Name ?? "",
        CourseName = s.Group?.Course?.Name ?? "",
        Day = s.Day.ToString(),
        StartTime = s.StartTime,
        EndTime = s.EndTime
    };
}