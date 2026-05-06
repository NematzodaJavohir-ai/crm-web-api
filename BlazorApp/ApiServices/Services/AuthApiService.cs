using System;
using BlazorApp.ApiServices.Interfaces;
using BlazorApp.DTOs.AuthDto;
using BlazorApp.DTOs.UserDto;

namespace BlazorApp.ApiServices.Services;

public class AuthApiService(HttpClient client) : IAuthApiService
{
    public Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckVerificationCodeAsync(VerifyCodeDto verifyCodeDto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        throw new NotImplementedException();
    }

    public Task<CreateUserResponseDto?> RegistrationAsync(CreateUserDto createUserDto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendEmailToResetPasswordAsync(SendEmailDto sendEmailDto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
