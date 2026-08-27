using System.Text.Json;
using Microsoft.JSInterop;
using TaskFlow.Web.Models;

namespace TaskFlow.Web.Authentication;

public sealed class SessionStorageService(IJSRuntime jsRuntime)
{
    private const string SessionKey = "taskflow.auth.session";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<AuthSession?> GetAsync()
    {
        var json = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", SessionKey);
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<AuthSession>(json, JsonOptions);
    }

    public ValueTask SetAsync(AuthSession session) =>
        jsRuntime.InvokeVoidAsync("sessionStorage.setItem", SessionKey, JsonSerializer.Serialize(session, JsonOptions));

    public ValueTask ClearAsync() => jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", SessionKey);
}
