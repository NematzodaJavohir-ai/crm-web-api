

using BlazorApp.DTOs.UserDto;

namespace BlazorApp.DTOs.AuthDto;

public record LoginResponseDto(
    GetUsersDto User,
    string Token
);

