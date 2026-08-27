using TaskFlow.src.Application.Authentication.Contracts;

namespace TaskFlow.src.Application.Authentication.Interfaces
{
    public interface ITokenService
    {
        AccessTokenResult CreateAccessToken(
            Guid userId,
            string email,
            string displayName,
            IReadOnlyCollection<string> roles);

        string GenerateRefreshToken();

        string HashRefreshToken(string refreshToken);
    }
}
