namespace Application.Dtos.UserDto;

public record GetUsersDto(
    int Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Role
);

