using System.ComponentModel.DataAnnotations;

namespace TaskFlow.src.Application.Authentication.Contracts;

public sealed record RefreshTokenRequest(
    [Required] string RefreshToken);
