using Application.Dtos.GroupDto;
using Application.Dtos.StudentDto;
using Application.Dtos.WeeklyResultDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;

namespace WebApi.Controllers;

[Route("api/students")]
public class StudentController(IStudentService studentService) : BaseController
{
    [HttpPost]
    ///[Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Create([FromBody] StudentCreateDto dto, CancellationToken ct)
    {
        var result = await studentService.CreateAsync(dto, ct);
        return HandleResult(result, 201);
    }

    [HttpPut("{id:int}")]
   // [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Update(int id, [FromBody] StudentUpdateDto dto, CancellationToken ct)
    {
        var result = await studentService.UpdateAsync(id, dto, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    //[Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await studentService.DeleteAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet]
    //[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await studentService.GetAllAsync(ct);
        return HandleResult(result);
    }

    [HttpPatch("{id:int}/set-active")]
    //[Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> SetActive(int id, [FromQuery] bool isActive, CancellationToken ct)
    {
        var result = await studentService.SetActiveAsync(id, isActive, ct);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId:int}")]
    //[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetByGroupId(int groupId, CancellationToken ct)
    {
        var result = await studentService.GetByGroupIdAsync(groupId, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:int}")]
    //[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await studentService.GetByIdAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:int}/with-groups")]
    //[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetWithGroups(int id, CancellationToken ct)
    {
        var result = await studentService.GetWithGroupsAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:int}/full-profile")]
    //[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Mentor))]
    public async Task<IActionResult> GetFullProfile(int id, CancellationToken ct)
    {
        var result = await studentService.GetFullProfileAsync(id, ct);
        return HandleResult(result);
    }

    [HttpGet("me")]
    //[Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var result = await studentService.GetMyProfileAsync(GetUserId(), ct);
        return HandleResult(result);
    }

    [HttpPut("me")]
    //[Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> UpdateMyProfile([FromBody] StudentUpdateDto dto, CancellationToken ct)
    {
        var result = await studentService.UpdateMyProfileAsync(GetUserId(), dto, ct);
        return HandleResult(result);
    }

    [HttpGet("me/full-profile")]
    //[Authorize(Roles = nameof(UserRole.Student))]
    public async Task<IActionResult> GetMyFullProfile(CancellationToken ct)
    {
        var result = await studentService.GetMyFullProfileAsync(GetUserId(), ct);
        return HandleResult(result);
    }
}