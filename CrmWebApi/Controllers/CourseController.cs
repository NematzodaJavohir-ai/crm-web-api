using Application.Dtos.CourseDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;

namespace WebApi.Controllers;

[Route("api/courses")]
public class CourseController(ICourseService courseService) : BaseController
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Create([FromBody] CourseCreateDto dto, CancellationToken ct)
    {
        var result = await courseService.CreateAsync(dto, ct);
        return HandleResult(result, 201);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Update(int id, [FromBody] CourseUpdateDto dto, CancellationToken ct)
    {
        var result = await courseService.UpdateAsync(id, dto, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await courseService.DeleteAsync(id, ct);
        return HandleResult(result);
    }

    [HttpPatch("{id:int}/set-active")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> SetActive(int id, [FromQuery] bool isActive, CancellationToken ct)
    {
        var result = await courseService.SetActiveAsync(id, isActive, ct);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await courseService.GetAllAsync(ct);
        return HandleResult(result);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllActive(CancellationToken ct)
    {
        var result = await courseService.GetAllActiveAsync(ct);
        return HandleResult(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await courseService.GetByIdAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:int}/with-groups")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetWithGroups(int id, CancellationToken ct)
    {
        var result = await courseService.GetWithGroupsAsync(id, ct);
        return HandleResult(result);
    }
}