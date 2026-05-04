using Application.Dtos.AttendanceDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;

namespace WebApi.Controllers;

[Route("api/attendances")]
public class AttendanceController(IAttendanceService attendanceService) : BaseController
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> Create([FromBody] AttendanceCreateDto dto, CancellationToken ct)
    {
        var result = await attendanceService.CreateAsync(dto, ct);
        return HandleResult(result, 201);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> Update(int id, [FromBody] AttendanceUpdateDto dto, CancellationToken ct)
    {
        var result = await attendanceService.UpdateAsync(id, dto, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await attendanceService.DeleteAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet("lesson/{lessonId:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetByLessonId(int lessonId, CancellationToken ct)
    {
        var result = await attendanceService.GetByLessonIdAsync(lessonId, ct);
        return HandleResult(result);
    }

    [HttpGet("week")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetByWeek([FromQuery] int groupId, [FromQuery] int weekNumber, CancellationToken ct)
    {
        var result = await attendanceService.GetByWeekAsync(groupId, weekNumber, ct);
        return HandleResult(result);
    }

    [HttpPost("{attendanceId:int}/absence-reason")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> AddAbsenceReason(int attendanceId, [FromBody] AddAbsenceReasonDto dto, CancellationToken ct)
    {
        var result = await attendanceService.AddAbsenceReasonAsync(attendanceId, GetUserId(), dto, ct);
        return HandleResult(result);
    }

    [HttpGet("me/group/{groupId:int}")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> GetMyAttendances(int groupId, CancellationToken ct)
    {
        var result = await attendanceService.GetMyAttendancesAsync(GetUserId(), groupId, ct);
        return HandleResult(result);
    }

    [HttpGet("me/group/{groupId:int}/average-score")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> GetMyAverageScore(int groupId, CancellationToken ct)
    {
        var result = await attendanceService.GetMyAverageScoreAsync(GetUserId(), groupId, ct);
        return HandleResult(result);
    }

    [HttpGet("me/group/{groupId:int}/absence-count")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> GetMyAbsenceCount(int groupId, CancellationToken ct)
    {
        var result = await attendanceService.GetMyAbsenceCountAsync(GetUserId(), groupId, ct);
        return HandleResult(result);
    }
}