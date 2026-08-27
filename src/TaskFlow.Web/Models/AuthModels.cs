using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Web.Models;

public sealed class LoginFormModel
{
    [Required(ErrorMessage = "Email address is required."), EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}

public sealed class RegisterFormModel
{
    [Required(ErrorMessage = "Display name is required."), MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required."), EmailAddress(ErrorMessage = "Enter a valid email address."), MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required."), MinLength(8, ErrorMessage = "Password must contain at least 8 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm your password."), Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public sealed record LoginRequest(string Email, string Password);
public sealed record RegisterRequest(string Email, string DisplayName, string Password, string ConfirmPassword);
public sealed record RefreshTokenRequest(string RefreshToken);
public sealed record CurrentUserResponse(Guid Id, string Email, string DisplayName);
public sealed record AuthResponse(string AccessToken, DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken, DateTimeOffset RefreshTokenExpiresAt, CurrentUserResponse User);
public sealed record AuthSession(string AccessToken, DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken, DateTimeOffset RefreshTokenExpiresAt, CurrentUserResponse User);

public sealed class ApiProblem
{
    public string? Title { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}
