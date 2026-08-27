using System.ComponentModel.DataAnnotations;

namespace TaskFlow.src.Application.Authentication.Contracts;

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
