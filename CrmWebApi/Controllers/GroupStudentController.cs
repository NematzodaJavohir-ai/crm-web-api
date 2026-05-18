using Application.Dtos.GroupStudentDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;

namespace WebApi.Controllers;

[Route("api/group-students")]
public class GroupStudentController(IGroupStudentService groupStudentService) : BaseController
{
    [HttpPost("add")]
    //[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> AddStudent([FromBody] AddStudentToGroupDto dto, CancellationToken ct)
    {
        var result = await groupStudentService.AddStudentAsync(dto, ct);
        return HandleResult(result, 201);
    }

    [HttpPost("remove")]
    //[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> RemoveStudent([FromBody] RemoveStudentFromGroupDto dto, CancellationToken ct)
    {
        var result = await groupStudentService.RemoveStudentAsync(dto, ct);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId:int}")]
    //[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetByGroupId(int groupId, CancellationToken ct)
    {
        var result = await groupStudentService.GetByGroupIdAsync(groupId, ct);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId:int}/active")]
    //[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetActiveByGroupId(int groupId, CancellationToken ct)
    {
        var result = await groupStudentService.GetActiveByGroupIdAsync(groupId, ct);
        return HandleResult(result);
    }

    [HttpGet("me/groups")]
    //[Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> GetMyGroups(CancellationToken ct)
    {
        var result = await groupStudentService.GetMyGroupsAsync(GetUserId(), ct);
        return HandleResult(result);
    }
    [HttpPost("transfer")]
    //[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Mentor)}")]
    public async Task<IActionResult> Transfer([FromBody] TransferStudentDto dto, CancellationToken ct)
    {
        var result = await groupStudentService.TransferStudentAsync(dto, ct);
        return HandleResult(result);
    }

    [HttpPost("restore/{groupId}/{studentId}")]
    //[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Mentor)}")]
    public async Task<IActionResult> Restore(int groupId, int studentId, CancellationToken ct)
    {
        var result = await groupStudentService.RestoreStudentAsync(groupId, studentId, ct);
        return HandleResult(result, 201);
    }

    [HttpGet("student/{studentId}/history")]
    //[Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetStudentHistory(int studentId, CancellationToken ct)
    {
        var result = await groupStudentService.GetStudentHistoryAsync(studentId, ct);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId}/history")]
   // [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Mentor)}")]
    public async Task<IActionResult> GetGroupHistory(int groupId, CancellationToken ct)
    {
        var result = await groupStudentService.GetGroupHistoryAsync(groupId, ct);
        return HandleResult(result);
    }

    [HttpPost("bulk-add")]
    //[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Mentor)}")]
    public async Task<IActionResult> BulkAdd([FromBody] BulkAddStudentsDto dto, CancellationToken ct)
    {
        var result = await groupStudentService.BulkAddStudentsAsync(dto, ct);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId}/stats")]
    //[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Mentor)}")]
    public async Task<IActionResult> GetGroupStats(int groupId, CancellationToken ct)
    {
        var result = await groupStudentService.GetGroupStatsAsync(groupId, ct);
        return HandleResult(result);
    }
}