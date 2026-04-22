using System.Transactions;
using Application.Dtos.AuthDto;
using Application.Dtos.UserDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AuthService(
IJwtService jwtService,
IEmailService emailService,
IUserRepository userRepository,
ILogger<AuthService> logger,
IVerificationCodeRepository verificationCodeRepository) : IAuthService
{
    public Task<string> ChangePasswordAync(ChangePasswordDto changePasswordDto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> ChekVerificationyCodeAsync(VerifyCodeDto verifyCodeDto, CancellationToken ct)
    {
        var user = await userRepository.GetUserByEmailAsync(verifyCodeDto.Email, ct);
        if (user == null)
        {
            return Result<bool>.Fail("User Not Found!", ErrorType.NotFound);
        }
        var latestCode = await verificationCodeRepository.GetLatestVerifyCode(user.Id, ct);

        if (latestCode == null)
        {
            return Result<bool>.Fail("No verification code found. Please request a new one.", ErrorType.Validation);
        }


        if (latestCode.Code != verifyCodeDto.Code)
        {
            return Result<bool>.Fail("Invalid verification code.", ErrorType.Validation);
        }
        if (latestCode.Expiration < DateTime.UtcNow)
        {
            return Result<bool>.Fail("Verification code has expired.", ErrorType.Validation);
        }
        return Result<bool>.Ok(true);
    }


    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await userRepository.GetUserByEmailAsync(loginDto.Email);
        if (user == null) return Result<LoginResponseDto>.Fail("Invalid Login Or Password!", ErrorType.Validation);

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return Result<LoginResponseDto>.Fail("Invalid Login Or Password!", ErrorType.Validation);
        }

        var token = jwtService.GenerateToken(user);

        var responseDto = new LoginResponseDto
      (
     User: new GetUsersDto
     (
         Id: user.Id,
         FirstName: user.FirstName,
         LastName: user.LastName,
         Email: user.Email,
         PhoneNumber: user.PhoneNumber,
         Role: user.Role.Name
     ),
     Token: token
    );

        return Result<LoginResponseDto>.Ok(responseDto);

    }

    public async Task<Result<CreateUserResponseDto>> RegistrationAsync(CreateUserDto createUserDto, CancellationToken ct)
    {
        if (await userRepository.EmailExistsAsync(createUserDto.Email, ct))
        {
            return Result<CreateUserResponseDto>.Fail($"User with email : {createUserDto.Email} alredy Exists!");
        }

        if (await userRepository.PhoneExistsAsync(createUserDto.PhoneNumber, ct))
        {
            return Result<CreateUserResponseDto>.Fail($"User with this phone: {createUserDto.PhoneNumber} alredy Exists!");
        }

        var passwordhash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

        var user = new User
        {
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Email = createUserDto.Email,
            PhoneNumber = createUserDto.PhoneNumber,
            PasswordHash = passwordhash,
            RoleId = (int)UserRole.Student
        };

        await userRepository.AddUserAsync(user);
        await userRepository.SaveChangesAsync(ct);

        var responseDto = new CreateUserResponseDto
        (
          Id: user.Id,
         FirstName: user.FirstName,
         LastName: user.LastName,
         Email: user.Email,
           PhoneNumber: user.PhoneNumber,
         Role: UserRole.Student.ToString()
        );

        return Result<CreateUserResponseDto>.Ok(responseDto);

    }

    public async Task<Result<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken ct)
    {
        var user = await userRepository.GetUserByEmailAsync(resetPasswordDto.Email);
        if (user == null) return Result<string>.Fail("User Not Found!", ErrorType.NotFound);


        var latestCode = await verificationCodeRepository.GetLatestVerifyCode(user.Id, ct);

        if (latestCode == null ||
        latestCode.Code != resetPasswordDto.Code ||
        latestCode.Expiration < DateTime.UtcNow)
        {
            return Result<string>.Fail("Invalid or expired verification code!", ErrorType.Validation);
        }
        if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
        {
            return Result<string>.Fail("Passwords do not match", ErrorType.Validation);
        }
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
        user.PasswordHash = passwordHash;
        await userRepository.UpdateUserAsync(user);

        await verificationCodeRepository.DeleteAllCodesByUserId(user.Id, ct);

        return Result<string>.Ok("Password reset successfully");


    }

    public async Task<Result<string>> SendEmailToResetPasswordAsync(SendEmailDto sendEmailDto, CancellationToken ct)
    {

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            var user = await userRepository.GetUserByEmailAsync(sendEmailDto.Email, ct);
            if (user == null) return Result<string>.Fail("User Not Found!", ErrorType.NotFound);

            await verificationCodeRepository.DeleteAllCodesByUserId(user.Id, ct);

            var code = new Random().Next(100000, 999999).ToString();

            var vfCode = new VerificationCode
            {
                UserId = user.Id,
                Code = code,
                CreatedAt = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddMinutes(5)
            };
            await verificationCodeRepository.AddVerifyCode(vfCode, ct);
            await verificationCodeRepository.SaveChangesAsync(ct);

            var sendEmail = await emailService.SendEmail(sendEmailDto.Email, "Password reset", $"<h3>{code} is your password reset code.</h3>");


            if (sendEmail)
            {
                transaction.Complete();
                return Result<string>.Ok("Succes!");
            }

            return Result<string>.Fail("Email failed", ErrorType.BadRequest);
        }
        catch (Exception ex)
        {

            logger.LogError(ex, "Reset password transaction failed for email: {Email}", sendEmailDto.Email);
            return Result<string>.Fail("An unexpected error occurred during the transaction.", ErrorType.BadRequest);

        }

    }

    public async Task<Result<CreateUserResponseDto>> CreateUserByAdminAsync(CreateUserByAdminDto dto, CancellationToken ct)
    {
        if (await userRepository.EmailExistsAsync(dto.Email, ct))
            return Result<CreateUserResponseDto>.Fail("User with this email already exists!");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var role = await userRepository.GetUserByRoleId(dto.RoleId, ct);

        if (role == null)
        {

            return Result<CreateUserResponseDto>.Fail("Selected role does not exist!", ErrorType.Validation);
        }


        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = passwordHash,
            RoleId = dto.RoleId
        };

        await userRepository.AddUserAsync(user);
        await userRepository.SaveChangesAsync(ct);


        return Result<CreateUserResponseDto>.Ok(new CreateUserResponseDto(
            Id: user.Id,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email,
            PhoneNumber: user.PhoneNumber,
            Role: ((UserRole)user.RoleId).ToString()
        ));
    }



}
