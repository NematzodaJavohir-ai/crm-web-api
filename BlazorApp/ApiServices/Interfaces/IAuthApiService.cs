using System;
using BlazorApp.DTOs.AuthDto;
using BlazorApp.DTOs.UserDto;

namespace BlazorApp.ApiServices.Interfaces;

public interface IAuthApiService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<CreateUserResponseDto?> RegistrationAsync(CreateUserDto createUserDto, CancellationToken ct = default);
    Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    Task<bool> SendEmailToResetPasswordAsync(SendEmailDto sendEmailDto, CancellationToken ct = default);
    Task<bool> CheckVerificationCodeAsync(VerifyCodeDto verifyCodeDto, CancellationToken ct = default);
    Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken ct = default);
}
