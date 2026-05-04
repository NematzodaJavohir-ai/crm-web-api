using Application.Dtos.AttendanceDto;
using Application.Dtos.LessonDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;

namespace WebApi.Controllers;

[Route("api/lessons")]
public class LessonController(ILessonService lessonService) : BaseController
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> Create([FromBody] LessonCreateDto dto, CancellationToken ct)
    {
        var result = await lessonService.CreateAsync(dto, ct);
        return HandleResult(result, 201);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> Update(int id, [FromBody] LessonUpdateDto dto, CancellationToken ct)
    {
        var result = await lessonService.UpdateAsync(id, dto, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await lessonService.DeleteAsync(id, ct);
        return HandleResult(result);
    }

    [HttpPatch("{id:int}/complete")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> MarkAsCompleted(int id, CancellationToken ct)
    {
        var result = await lessonService.MarkAsCompletedAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetByGroupId(int groupId, CancellationToken ct)
    {
        var result = await lessonService.GetByGroupIdAsync(groupId, ct);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId:int}/week/{weekNumber:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetByWeekNumber(int groupId, int weekNumber, CancellationToken ct)
    {
        var result = await lessonService.GetByWeekNumberAsync(groupId, weekNumber, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:int}/with-attendances")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetWithAttendances(int id, CancellationToken ct)
    {
        var result = await lessonService.GetWithAttendancesAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet("me/group/{groupId:int}")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> GetMyLessons(int groupId, CancellationToken ct)
    {
        var result = await lessonService.GetMyLessonsAsync(GetUserId(), groupId, ct);
        return HandleResult(result);
    }
}