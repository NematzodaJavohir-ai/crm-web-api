namespace BlazorApp.DTOs.AuthDto;

public record CreateUserByAdminDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    int RoleId 
);
