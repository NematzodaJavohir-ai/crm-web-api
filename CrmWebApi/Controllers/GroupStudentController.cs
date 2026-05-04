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
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> AddStudent([FromBody] AddStudentToGroupDto dto, CancellationToken ct)
    {
        var result = await groupStudentService.AddStudentAsync(dto, ct);
        return HandleResult(result, 201);
    }

    [HttpPost("remove")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> RemoveStudent([FromBody] RemoveStudentFromGroupDto dto, CancellationToken ct)
    {
        var result = await groupStudentService.RemoveStudentAsync(dto, ct);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetByGroupId(int groupId, CancellationToken ct)
    {
        var result = await groupStudentService.GetByGroupIdAsync(groupId, ct);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId:int}/active")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetActiveByGroupId(int groupId, CancellationToken ct)
    {
        var result = await groupStudentService.GetActiveByGroupIdAsync(groupId, ct);
        return HandleResult(result);
    }

    [HttpGet("me/groups")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> GetMyGroups(CancellationToken ct)
    {
        var result = await groupStudentService.GetMyGroupsAsync(GetUserId(), ct);
        return HandleResult(result);
    }
}