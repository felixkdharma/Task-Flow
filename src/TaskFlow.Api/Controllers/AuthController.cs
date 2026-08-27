using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.src.Application.Authentication.Contracts;
using TaskFlow.src.Application.Authentication.Interfaces;
using TaskFlow.src.Infrastructure.Authentication;

namespace TaskFlow.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAuthService authService,
    UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.RegisterAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (AuthServiceException exception)
        {
            return ToProblem(exception);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await authService.LoginAsync(request, cancellationToken));
        }
        catch (AuthServiceException exception)
        {
            return ToProblem(exception);
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await authService.RefreshAsync(request, cancellationToken));
        }
        catch (AuthServiceException exception)
        {
            return ToProblem(exception);
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<CurrentUserResponse>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = userId is null ? null : await userManager.FindByIdAsync(userId);
        if (user is null || !user.IsActive)
        {
            return Unauthorized();
        }

        return Ok(new CurrentUserResponse(user.Id, user.Email!, user.DisplayName));
    }

    private ObjectResult ToProblem(AuthServiceException exception)
    {
        ProblemDetails problem = exception.Errors is null
            ? new ProblemDetails()
            : new ValidationProblemDetails(exception.Errors.ToDictionary(pair => pair.Key, pair => pair.Value));
        problem.Status = exception.StatusCode;
        problem.Title = exception.Title;
        problem.Instance = HttpContext.Request.Path;
        return StatusCode(exception.StatusCode, problem);
    }
}
