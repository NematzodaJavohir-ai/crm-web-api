using Application.Dtos.GroupDto;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/groups")]
public class GroupController(IGroupService groupService) : BaseController
{
    // ───── Admin ─────

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Create([FromBody] GroupCreateDto dto, CancellationToken ct)
    {
        var result = await groupService.CreateAsync(dto, ct);
        return HandleResult(result, 201);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Update(int id, [FromBody] GroupUpdateDto dto, CancellationToken ct)
    {
        var result = await groupService.UpdateAsync(id, dto, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await groupService.DeleteAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await groupService.GetAllAsync(ct);
        return HandleResult(result);
    }

    [HttpGet("active")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetAllActive(CancellationToken ct)
    {
        var result = await groupService.GetAllActiveAsync(ct);
        return HandleResult(result);
    }

    [HttpGet("mentor/{mentorId:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Mentor)}")]
    public async Task<IActionResult> GetByMentorId(int mentorId, CancellationToken ct)
    {
        var result = await groupService.GetByMentorIdAsync(mentorId, ct);
        return HandleResult(result);
    }

    [HttpGet("course/{courseId:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetByCourseId(int courseId, CancellationToken ct)
    {
        var result = await groupService.GetByCourseIdAsync(courseId, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Mentor)}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await groupService.GetByIdAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:int}/with-students")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Mentor)}")]
    public async Task<IActionResult> GetWithStudents(int id, CancellationToken ct)
    {
        var result = await groupService.GetWithStudentsAsync(id, ct);
        return HandleResult(result);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> ChangeStatus(int id, [FromQuery] GroupStatus status, CancellationToken ct)
    {
        var result = await groupService.ChangeStatusAsync(id, status, ct);
        return HandleResult(result);
    }
}