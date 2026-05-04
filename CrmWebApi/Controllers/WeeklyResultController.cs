using Application.Dtos.WeeklyResultDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;

namespace WebApi.Controllers;

[Route("api/week-results")]
public class WeekResultController(IWeekResultService weekResultService) : BaseController
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> Create([FromBody] WeeklyResultCreateDto dto, CancellationToken ct)
    {
        var result = await weekResultService.CreateAsync(dto, ct);
        return HandleResult(result, 201);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> Update(int id, [FromBody] WeeklyResultUpdateDto dto, CancellationToken ct)
    {
        var result = await weekResultService.UpdateAsync(id, dto, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await weekResultService.DeleteAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId:int}/week/{weekNumber:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetByGroupAndWeek(int groupId, int weekNumber, CancellationToken ct)
    {
        var result = await weekResultService.GetByGroupAndWeekAsync(groupId, weekNumber, ct);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetByGroupId(int groupId, CancellationToken ct)
    {
        var result = await weekResultService.GetByGroupIdAsync(groupId, ct);
        return HandleResult(result);
    }

    [HttpPost("recalculate")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> RecalculateTotal([FromQuery] int studentId, [FromQuery] int groupId, [FromQuery] int weekNumber, CancellationToken ct)
    {
        var result = await weekResultService.RecalculateTotalAsync(studentId, groupId, weekNumber, ct);
        return HandleResult(result);
    }

    [HttpGet("me/group/{groupId:int}")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> GetMyResults(int groupId, CancellationToken ct)
    {
        var result = await weekResultService.GetMyResultsAsync(GetUserId(), groupId, ct);
        return HandleResult(result);
    }

    [HttpGet("me/group/{groupId:int}/best-week")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> GetMyBestWeek(int groupId, CancellationToken ct)
    {
        var result = await weekResultService.GetMyBestWeekAsync(GetUserId(), groupId, ct);
        return HandleResult(result);
    }

    [HttpGet("me/group/{groupId:int}/average-score")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> GetMyAverageScore(int groupId, CancellationToken ct)
    {
        var result = await weekResultService.GetMyAverageScoreAsync(GetUserId(), groupId, ct);
        return HandleResult(result);
    }
}