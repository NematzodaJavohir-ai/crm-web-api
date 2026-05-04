using Application.Results;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected int GetUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    protected string GetUserRole()
        => User.FindFirstValue(ClaimTypes.Role)!;

    protected IActionResult HandleResult<T>(Result<T> result, int successStatusCode = 200)
    {
        if (result is null)
            return StatusCode(500, "Result is null");

        if (result.IsSuccess)
            return StatusCode(successStatusCode, result.Data);

        return result.ErrorType switch
        {
            ErrorType.NotFound     => NotFound(result.Error),
            ErrorType.Conflict     => Conflict(result.Error),
            ErrorType.Validation   => BadRequest(result.Error),
            ErrorType.Unauthorized => Unauthorized(result.Error),
            ErrorType.Forbidden    => StatusCode(403, result.Error),
            _                      => StatusCode(500, result.Error)
        };
    }
}