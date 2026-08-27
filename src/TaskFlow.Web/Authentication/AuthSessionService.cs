using System.Net;
using System.Net.Http.Json;
using TaskFlow.Web.Models;

namespace TaskFlow.Web.Authentication;

public sealed class AuthSessionService(
    IHttpClientFactory httpClientFactory,
    SessionStorageService storage,
    TaskFlowAuthenticationStateProvider authenticationStateProvider)
{
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private AuthSession? _session;

    public bool IsAuthenticated => _session is not null && _session.RefreshTokenExpiresAt > DateTimeOffset.UtcNow;
    public CurrentUserResponse? CurrentUser => _session?.User;

    public async Task RestoreAsync()
    {
        try
        {
            _session = await storage.GetAsync();
            if (_session is null || _session.RefreshTokenExpiresAt <= DateTimeOffset.UtcNow)
            {
                await ClearAsync();
                return;
            }

            if (_session.AccessTokenExpiresAt <= DateTimeOffset.UtcNow.AddSeconds(30))
            {
                await TryRefreshAsync(force: true);
                return;
            }

            authenticationStateProvider.SetSession(_session);
        }
        catch
        {
            await ClearAsync();
        }
    }

    public Task LoginAsync(LoginFormModel model) =>
        AuthenticateAsync("api/auth/login", new LoginRequest(model.Email.Trim(), model.Password));

    public Task RegisterAsync(RegisterFormModel model) =>
        AuthenticateAsync("api/auth/register", new RegisterRequest(
            model.Email.Trim(), model.DisplayName.Trim(), model.Password, model.ConfirmPassword));

    public async Task<string?> GetValidAccessTokenAsync()
    {
        if (_session is null)
        {
            return null;
        }

        if (_session.AccessTokenExpiresAt > DateTimeOffset.UtcNow.AddSeconds(30))
        {
            return _session.AccessToken;
        }

        return await TryRefreshAsync(force: false) ? _session?.AccessToken : null;
    }

    public async Task<bool> TryRefreshAsync(bool force)
    {
        await _refreshLock.WaitAsync();
        try
        {
            if (_session is null || _session.RefreshTokenExpiresAt <= DateTimeOffset.UtcNow)
            {
                await ClearAsync();
                return false;
            }

            if (!force && _session.AccessTokenExpiresAt > DateTimeOffset.UtcNow.AddSeconds(30))
            {
                return true;
            }

            var client = httpClientFactory.CreateClient("AuthenticationApi");
            var response = await client.PostAsJsonAsync(
                "api/auth/refresh",
                new RefreshTokenRequest(_session.RefreshToken));
            if (!response.IsSuccessStatusCode)
            {
                await ClearAsync();
                return false;
            }

            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (auth is null)
            {
                await ClearAsync();
                return false;
            }

            await SaveAsync(auth);
            return true;
        }
        catch
        {
            await ClearAsync();
            return false;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<CurrentUserResponse?> GetCurrentUserAsync()
    {
        var client = httpClientFactory.CreateClient("TaskFlowApi");
        var response = await client.GetAsync("api/auth/me");
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<CurrentUserResponse>()
            : null;
    }

    public Task LogoutAsync() => ClearAsync();

    private async Task AuthenticateAsync<TRequest>(string endpoint, TRequest request)
    {
        var client = httpClientFactory.CreateClient("AuthenticationApi");
        var response = await client.PostAsJsonAsync(endpoint, request);
        if (!response.IsSuccessStatusCode)
        {
            throw await CreateExceptionAsync(response);
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new AuthApiException(500, "The server returned an invalid authentication response.");
        await SaveAsync(auth);
    }

    private async Task SaveAsync(AuthResponse response)
    {
        _session = new AuthSession(response.AccessToken, response.AccessTokenExpiresAt,
            response.RefreshToken, response.RefreshTokenExpiresAt, response.User);
        await storage.SetAsync(_session);
        authenticationStateProvider.SetSession(_session);
    }

    private async Task ClearAsync()
    {
        _session = null;
        await storage.ClearAsync();
        authenticationStateProvider.SetSession(null);
    }

    private static async Task<AuthApiException> CreateExceptionAsync(HttpResponseMessage response)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ApiProblem>();
            var detail = problem?.Errors?.Values.SelectMany(value => value).FirstOrDefault();
            return new AuthApiException((int)response.StatusCode,
                detail ?? problem?.Title ?? "The request could not be completed.");
        }
        catch
        {
            return new AuthApiException((int)response.StatusCode,
                response.StatusCode == HttpStatusCode.Unauthorized
                    ? "Your email or password is incorrect."
                    : "The request could not be completed.");
        }
    }
}
