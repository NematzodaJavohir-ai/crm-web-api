using Application.Dtos.UserDto;

namespace Application.Dtos.AuthDto;

public record LoginResponseDto(
    GetUsersDto User,
    string Token
);

