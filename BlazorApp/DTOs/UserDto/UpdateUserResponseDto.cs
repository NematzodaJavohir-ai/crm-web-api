namespace BlazorApp.DTOs.UserDto;

public record UpdateUserResponseDto(
    int Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Role
);

