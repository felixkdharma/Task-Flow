using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskFlow.Web;
using TaskFlow.Web.Authentication;
using TaskFlow.Web.Projects;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl is not configured.");

//builder.Services.AddAuthorizationCore();
//builder.Services.AddScoped<SessionStorageService>();
//builder.Services.AddScoped<TaskFlowAuthenticationStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
//    provider.GetRequiredService<TaskFlowAuthenticationStateProvider>());
//builder.Services.AddScoped<AuthSessionService>();
//builder.Services.AddScoped<ProjectApiService>();
//builder.Services.AddTransient<ApiAuthorizationHandler>();
//builder.Services.AddHttpClient("AuthenticationApi", client => client.BaseAddress = new Uri(apiBaseUrl));
//builder.Services.AddHttpClient("TaskFlowApi", client => client.BaseAddress = new Uri(apiBaseUrl))
//    .AddHttpMessageHandler<ApiAuthorizationHandler>();

builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<SessionStorageService>();
builder.Services.AddSingleton<TaskFlowAuthenticationStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<TaskFlowAuthenticationStateProvider>());
builder.Services.AddSingleton<AuthSessionService>();

builder.Services.AddScoped<ProjectApiService>();
builder.Services.AddTransient<ApiAuthorizationHandler>();

builder.Services.AddHttpClient(
    "AuthenticationApi",
    client => client.BaseAddress = new Uri(apiBaseUrl));

builder.Services.AddHttpClient(
        "TaskFlowApi",
        client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<ApiAuthorizationHandler>();

await builder.Build().RunAsync();
