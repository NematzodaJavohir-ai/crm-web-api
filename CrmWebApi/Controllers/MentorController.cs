using Application.Dtos.MentorDto;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/mentors")]
public class MentorController(IMentorService mentorService) : BaseController
{
    // ───── Admin ─────

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Create([FromBody] MentorCreateDto dto, CancellationToken ct)
    {
        var result = await mentorService.CreateAsync(dto, ct);
        return HandleResult(result, 201);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await mentorService.DeleteAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mentorService.GetAllAsync(ct);
        return HandleResult(result);
    }

    [HttpGet("active")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetAllActive(CancellationToken ct)
    {
        var result = await mentorService.GetAllActiveAsync(ct);
        return HandleResult(result);
    }

    [HttpPatch("{id:int}/set-active")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> SetActive(int id, [FromQuery] bool isActive, CancellationToken ct)
    {
        var result = await mentorService.SetActiveAsync(id, isActive, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Mentor)}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await mentorService.GetByIdAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:int}/with-groups")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Mentor)}")]
    public async Task<IActionResult> GetWithGroups(int id, CancellationToken ct)
    {
        var result = await mentorService.GetWithGroupsAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet("me")]
    [Authorize(Roles = nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var result = await mentorService.GetMyProfileAsync(GetUserId(), ct);
        return HandleResult(result);
    }

    [HttpPut("me")]
    [Authorize(Roles = nameof(UserRole.Mentor))]
    public async Task<IActionResult> UpdateMyProfile([FromBody] MentorUpdateDto dto, CancellationToken ct)
    {
        var result = await mentorService.UpdateMyProfileAsync(GetUserId(), dto, ct);
        return HandleResult(result);
    }
}