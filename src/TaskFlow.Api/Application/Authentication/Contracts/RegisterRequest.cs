using System.ComponentModel.DataAnnotations;

namespace TaskFlow.src.Application.Authentication.Contracts;

public sealed record RegisterRequest(
    [Required, EmailAddress, MaxLength(320)] string Email,
    [Required, MaxLength(100)] string DisplayName,
    [Required] string Password,
    [Required] string ConfirmPassword);
