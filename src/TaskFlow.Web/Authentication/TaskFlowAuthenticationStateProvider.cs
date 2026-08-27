using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using TaskFlow.Web.Models;

namespace TaskFlow.Web.Authentication;

public sealed class TaskFlowAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private AuthenticationState _currentState = Anonymous;

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(_currentState);

    public void SetSession(AuthSession? session)
    {
        _currentState = session is null
            ? Anonymous
            : new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, session.User.Id.ToString()),
                new Claim(ClaimTypes.Name, session.User.DisplayName),
                new Claim(ClaimTypes.Email, session.User.Email)
            ], "TaskFlowJwt")));
        NotifyAuthenticationStateChanged(Task.FromResult(_currentState));
    }
}
