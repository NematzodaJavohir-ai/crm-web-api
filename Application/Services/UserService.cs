using Application.Dtos.UserDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Enums;

namespace Application.Services;

public class UserService(IUserRepository repository) : IUserService
{
    public async Task<Result<bool>> DeleteUserAsync(int id,CancellationToken ct = default)
    {

        var user = await repository.GetUserByIdAsync(id,ct);
        if (user == null) return Result<bool>.Fail("User Not Found!", ErrorType.NotFound);

        var deleted = await repository.DeleteUserAsync(id,ct);
        if(!deleted)return Result<bool>.Fail("Delete failed", ErrorType.BadRequest);
        await repository.SaveChangesAsync(ct);

        return Result<bool>.Ok(await repository.DeleteUserAsync(id,ct));

    }
    public async Task<Result<IEnumerable<GetUsersDto>>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await repository.GetUsersAsync();

        var userDto = users.Select(us => new GetUsersDto(

           Id: us.Id,
           FirstName: us.FirstName,
           LastName: us.LastName,
           PhoneNumber: us.PhoneNumber,
           Email: us.Email,
           Role: us.Role?.Name ?? "No Role"

        )).ToList();

        return Result<IEnumerable<GetUsersDto>>.Ok(userDto);
    }

    public async Task<Result<GetUsersDto>> GetUserByIdAsync(int id,CancellationToken ct = default)
    {
       var user = await repository.GetUserByIdAsync(id,ct);
       if(user== null)return Result<GetUsersDto>.Fail("User Not Found!",ErrorType.NotFound);

       var userDto = new GetUsersDto(

           Id: user.Id,
           FirstName: user.FirstName,
           LastName: user.LastName,
           PhoneNumber: user.PhoneNumber,
           Email: user.Email,
           Role: user.Role?.Name ?? "No Role"

        );

        return Result<GetUsersDto>.Ok(userDto);


    }
   public async Task<Result<UpdateUserResponseDto>> UpdateUserAsync(int id, UpdateUserDto dto, CancellationToken ct = default)
{
    var user = await repository.GetUserByIdAsync(id, ct);
    if (user == null) return Result<UpdateUserResponseDto>.Fail("User Not Found!", ErrorType.NotFound);

    var roleName = user.Role?.Name ?? "No Role";

    if (user.PhoneNumber != dto.PhoneNumber && await repository.PhoneExistsAsync(dto.PhoneNumber, ct))
        return Result<UpdateUserResponseDto>.Fail("Phone number is already Taken", ErrorType.Conflict);
    if (user.Email != dto.Email && await repository.EmailExistsAsync(dto.Email))
        return Result<UpdateUserResponseDto>.Fail("Email is already Taken", ErrorType.Conflict);

    user.FirstName = dto.FirstName.Trim();
    user.LastName = dto.LastName.Trim();
    user.Email = dto.Email.Trim().ToLower();
    user.PhoneNumber = dto.PhoneNumber.Trim();

    await repository.UpdateUserAsync(user, ct);
    await repository.SaveChangesAsync(ct);

    var response = new UpdateUserResponseDto(
        Id: user.Id,
        FirstName: user.FirstName,
        LastName: user.LastName,
        PhoneNumber: user.PhoneNumber,
        Email: user.Email,
        Role: roleName 
    );

    return Result<UpdateUserResponseDto>.Ok(response);
}

}
