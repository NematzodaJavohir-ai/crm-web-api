using Application.Exeptions;
using Application.Results;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result == null) return StatusCode(500, "Result is null");

        if (result.IsSuccess)
        {
        
            if (result.Data is null || (result.Data is bool b && b == true)) 
                return Ok(result); 

            return Ok(result);
        }

        
        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(result),
            ErrorType.Conflict => Conflict(result), 
            ErrorType.Validation => BadRequest(result),
            ErrorType.Unauthorized => Unauthorized(result),
            _ => StatusCode(500, result)
        };
    }
}