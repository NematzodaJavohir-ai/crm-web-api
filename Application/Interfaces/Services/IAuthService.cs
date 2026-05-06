using Application.Dtos.AuthDto;
using Application.Dtos.UserDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IAuthService
{
   Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto);
   Task<Result<CreateUserResponseDto>> RegistrationAsync(CreateUserDto createUserDto, CancellationToken ct);
   Task<string>ChangePasswordAync(ChangePasswordDto changePasswordDto);
   Task<Result<string>> SendEmailToResetPasswordAsync(SendEmailDto sendEmailDto, CancellationToken ct);
   Task<Result<bool>> ChekVerificationyCodeAsync(VerifyCodeDto verifyCodeDto, CancellationToken ct);
   Task<Result<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken ct);
   Task<Result<CreateUserResponseDto>> CreateUserByAdminAsync(CreateUserByAdminDto dto, CancellationToken ct);

}
