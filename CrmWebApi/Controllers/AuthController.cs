using Application.Dtos.AuthDto;
using Application.Dtos.UserDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;

namespace CrmWebApi.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService service):BaseController
{
    [HttpPost("login")]
    public async Task<IActionResult>LoginAsync(LoginDto dto)
    {
        return HandleResult(await service.LoginAsync(dto));
    }
    [HttpPost("registration")]
    public async Task<IActionResult>Registration(CreateUserDto dto,CancellationToken ct)
    {
        return HandleResult(await service.RegistrationAsync(dto,ct));
    }

    [HttpPost("send-reset-password-to-email")]
    public async Task<IActionResult>SendResetPasswordToEmail(SendEmailDto dto,CancellationToken ct)
    {
        return HandleResult(await service.SendEmailToResetPasswordAsync(dto,ct));
    }

     [HttpPost("check-verification-code")]
    public async Task<IActionResult>CheckVerificationCode(VerifyCodeDto dto,CancellationToken ct)
    {
        return HandleResult(await service.ChekVerificationyCodeAsync(dto,ct));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult>ResetPassword(ResetPasswordDto dto,CancellationToken ct)
    {
        return HandleResult(await service.ResetPasswordAsync(dto,ct));
    }

    [HttpPost("create-user-by-admin")]
    [Authorize(Roles ="Admin")]
    public async Task<IActionResult>CreateUserByAdmin(CreateUserByAdminDto dto,CancellationToken ct)
    {
        return HandleResult(await service.CreateUserByAdminAsync(dto,ct));
    }

}
