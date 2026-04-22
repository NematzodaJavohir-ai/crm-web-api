namespace Application.Dtos.UserDto;

public record CreateUserResponseDto
(
    int Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Role
);

