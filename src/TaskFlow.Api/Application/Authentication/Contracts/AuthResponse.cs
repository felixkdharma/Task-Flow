namespace TaskFlow.src.Application.Authentication.Contracts
{
    public sealed record AuthResponse(
        string AccessToken,
        DateTimeOffset AccessTokenExpiresAt,
        string RefreshToken,
        DateTimeOffset RefreshTokenExpiresAt,
        CurrentUserResponse User
    );
}
