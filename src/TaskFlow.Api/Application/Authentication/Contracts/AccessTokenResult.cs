namespace TaskFlow.src.Application.Authentication.Contracts
{
    public sealed record AccessTokenResult(
        string Token,
        DateTimeOffset ExpiresAt);
}
