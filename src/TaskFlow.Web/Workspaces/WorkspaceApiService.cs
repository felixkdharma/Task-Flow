using System.Net;
using System.Net.Http.Json;
using TaskFlow.Web.Models;

namespace TaskFlow.Web.Workspaces;

public sealed class WorkspaceApiService(IHttpClientFactory httpClientFactory)
{
    public async Task<IReadOnlyList<WorkspaceResponse>> GetWorkspacesAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        using var response = await Client.GetAsync(
            $"api/workspace?projectId={projectId}", cancellationToken);
        await ThrowIfFailedAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<List<WorkspaceResponse>>(cancellationToken)
            ?? [];
    }

    public async Task<WorkspaceResponse> CreateWorkspaceAsync(
        Guid projectId,
        Guid userId,
        WorkspaceFormModel model,
        CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(projectId, userId, model);
        using var response = await Client.PostAsJsonAsync("api/workspace", request, cancellationToken);
        await ThrowIfFailedAsync(response, cancellationToken);
        return await ReadWorkspaceAsync(response, cancellationToken);
    }

    public async Task<WorkspaceResponse> UpdateWorkspaceAsync(
        Guid workspaceId,
        Guid projectId,
        Guid userId,
        WorkspaceFormModel model,
        CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(projectId, userId, model);
        using var response = await Client.PutAsJsonAsync(
            $"api/workspace/{workspaceId}?workspaceId={workspaceId}", request, cancellationToken);
        await ThrowIfFailedAsync(response, cancellationToken);
        return await ReadWorkspaceAsync(response, cancellationToken);
    }

    public async Task<bool> DeleteWorkspaceAsync(
        Guid projectId,
        Guid workspaceId,
        CancellationToken cancellationToken = default)
    {
        using var response = await Client.DeleteAsync(
            $"api/workspace/{workspaceId}?projectId={projectId}&workspaceId={workspaceId}",
            cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        await ThrowIfFailedAsync(response, cancellationToken);
        return true;
    }

    private HttpClient Client => httpClientFactory.CreateClient("TaskFlowApi");

    private static WorkspaceRequest CreateRequest(Guid projectId, Guid userId, WorkspaceFormModel model) =>
        new(model.WorkspaceName.Trim(), model.WorkspaceDescription.Trim(), projectId, userId);

    private static async Task<WorkspaceResponse> ReadWorkspaceAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken) =>
        await response.Content.ReadFromJsonAsync<WorkspaceResponse>(cancellationToken)
        ?? throw new WorkspaceApiException(500, "The server returned an invalid workspace response.");

    private static async Task ThrowIfFailedAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ApiProblem>(cancellationToken);
            var detail = problem?.Errors?.Values.SelectMany(value => value).FirstOrDefault();
            throw new WorkspaceApiException(
                (int)response.StatusCode,
                detail ?? problem?.Title ?? "The workspace request could not be completed.");
        }
        catch (WorkspaceApiException)
        {
            throw;
        }
        catch
        {
            throw new WorkspaceApiException(
                (int)response.StatusCode,
                "The workspace request could not be completed.");
        }
    }
}
