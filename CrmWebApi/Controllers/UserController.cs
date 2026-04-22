using Application.Dtos.UserDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;


namespace CrmWebApi.Controllers;
[ApiController]
[Route("api/users")]
[Authorize]
public class UserController(IUserService service):BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetUsersAsync(CancellationToken ct)
    {
        return HandleResult(await service.GetAllUsersAsync(ct));

    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id,CancellationToken ct)
    {
       return HandleResult(await service.GetUserByIdAsync(id,ct));
    }

    [HttpDelete("{id:int}")]

     public async Task<IActionResult> DeleteUserAsync(int id,CancellationToken ct)
    {
       return HandleResult(await service.DeleteUserAsync(id,ct));
    }

     [HttpPut("{id:int}")]

     public async Task<IActionResult> UpdateUserAsync(int id,UpdateUserDto dto,CancellationToken ct)
    {
       return HandleResult(await service.UpdateUserAsync(id,dto,ct));
    }
    
}
