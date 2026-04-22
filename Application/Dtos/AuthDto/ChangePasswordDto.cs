using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.AuthDto;

public record ChangePasswordDto(
    [property: Required(ErrorMessage = "Old password is required.")]
    string OldPassword,

    [property: Required(ErrorMessage = "New password is required.")]
    [property: StringLength(50, MinimumLength = 8, ErrorMessage = "New password must be at least 8 characters long.")]
    string NewPassword,

    [property: Required(ErrorMessage = "Confirm password is required.")]
    [property: Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    string ConfirmPassword
);