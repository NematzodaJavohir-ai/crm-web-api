using Application.Dtos.UserDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IUserService
{
  Task<Result<IEnumerable<GetUsersDto>>> GetAllUsersAsync(CancellationToken ct = default);
  Task<Result<GetUsersDto>>GetUserByIdAsync(int id,CancellationToken cancellationToken);
  Task<Result<UpdateUserResponseDto>>UpdateUserAsync(int id,UpdateUserDto dto,CancellationToken ct = default);
  Task<Result<bool>>DeleteUserAsync(int id,CancellationToken cancellationToken);
}
