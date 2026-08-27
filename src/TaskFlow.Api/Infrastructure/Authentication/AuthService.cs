using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskFlow.src.Application.Authentication.Contracts;
using TaskFlow.src.Application.Authentication.Interfaces;
using TaskFlow.src.Domain.Entities;
using TaskFlow.src.Persistence;

namespace TaskFlow.src.Infrastructure.Authentication;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ApplicationDbContext dbContext,
    ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            throw BadRequest("Registration validation failed.", "confirmPassword", "Passwords do not match.");
        }

        var email = request.Email.Trim();
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            throw new AuthServiceException(StatusCodes.Status409Conflict, "An account with this email already exists.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var now = DateTimeOffset.UtcNow;
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            DisplayName = request.DisplayName.Trim(),
            IsActive = true,
            CreatedAt = now
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var duplicate = result.Errors.Any(error =>
                error.Code is "DuplicateEmail" or "DuplicateUserName");
            var errors = result.Errors
                .GroupBy(error => "registration", StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());
            throw new AuthServiceException(
                duplicate ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest,
                duplicate ? "An account with this email already exists." : "Registration validation failed.",
                errors);
        }

        var response = await IssueTokensAsync(user, now, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return response;
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null || !user.IsActive)
        {
            throw Unauthorized();
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            throw Unauthorized();
        }

        return await IssueTokensAsync(user, DateTimeOffset.UtcNow, cancellationToken);
    }

    public async Task<AuthResponse> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var storedToken = await dbContext.RefreshTokens
            .Include(token => token.User)
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        var now = DateTimeOffset.UtcNow;
        if (storedToken is null || !storedToken.IsActive || !storedToken.User.IsActive)
        {
            throw Unauthorized("The refresh token is invalid or expired.");
        }

        var replacementToken = tokenService.GenerateRefreshToken();
        var replacementHash = tokenService.HashRefreshToken(replacementToken);
        storedToken.RevokedAt = now;
        storedToken.UpdatedAt = now;
        storedToken.ReplacedByTokenHash = replacementHash;

        var response = await IssueTokensAsync(
            storedToken.User,
            now,
            cancellationToken,
            replacementToken,
            replacementHash);
        await transaction.CommitAsync(cancellationToken);
        return response;
    }

    private async Task<AuthResponse> IssueTokensAsync(
        ApplicationUser user,
        DateTimeOffset now,
        CancellationToken cancellationToken,
        string? refreshToken = null,
        string? refreshTokenHash = null)
    {
        var roles = await userManager.GetRolesAsync(user);
        var accessToken = tokenService.CreateAccessToken(
            user.Id,
            user.Email!,
            user.DisplayName,
            roles.ToArray());
        refreshToken ??= tokenService.GenerateRefreshToken();
        refreshTokenHash ??= tokenService.HashRefreshToken(refreshToken);
        var refreshExpiresAt = now.AddDays(_jwtOptions.RefreshTokenDays);

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            CreatedAt = now,
            ExpiresAt = refreshExpiresAt
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            accessToken.Token,
            accessToken.ExpiresAt,
            refreshToken,
            refreshExpiresAt,
            new CurrentUserResponse(user.Id, user.Email!, user.DisplayName));
    }

    private static AuthServiceException Unauthorized(string title = "Invalid email or password.") =>
        new(StatusCodes.Status401Unauthorized, title);

    private static AuthServiceException BadRequest(string title, string field, string error) =>
        new(StatusCodes.Status400BadRequest, title,
            new Dictionary<string, string[]> { [field] = [error] });
}
