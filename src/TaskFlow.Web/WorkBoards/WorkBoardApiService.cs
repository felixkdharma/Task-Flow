using System.Net.Http.Json;
using TaskFlow.Web.Models;

namespace TaskFlow.Web.WorkBoards;

public sealed class WorkBoardApiService(IHttpClientFactory httpClientFactory)
{
    public async Task<IReadOnlyList<WorkBoardResponse>> GetWorkBoardsAsync(
        Guid projectId,
        Guid workspaceId,
        CancellationToken cancellationToken = default)
    {
        using var response = await Client.GetAsync(
            $"api/workboard?projectId={projectId}&workspaceId={workspaceId}",
            cancellationToken);
        await ThrowIfFailedAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<List<WorkBoardResponse>>(cancellationToken) ?? [];
    }

    public async Task<WorkBoardResponse> CreateWorkBoardAsync(
        Guid projectId,
        Guid workspaceId,
        int status,
        WorkBoardFormModel model,
        CancellationToken cancellationToken = default)
    {
        var request = new WorkBoardCreateRequest(
            model.WorkBoardName.Trim(),
            model.WorkBoardDescription.Trim(),
            status,
            model.StartDate.Date,
            model.EndDate.Date,
            projectId,
            workspaceId);
        using var response = await Client.PostAsJsonAsync("api/workboard", request, cancellationToken);
        await ThrowIfFailedAsync(response, cancellationToken);
        return await ReadWorkBoardAsync(response, cancellationToken);
    }

    public async Task<WorkBoardResponse> UpdateWorkBoardAsync(
        Guid workBoardId,
        Guid projectId,
        Guid workspaceId,
        WorkBoardFormModel model,
        CancellationToken cancellationToken = default)
    {
        var request = new WorkBoardDetailsUpdateRequest(
            model.WorkBoardName.Trim(),
            model.WorkBoardDescription.Trim(),
            model.StartDate.Date,
            model.EndDate.Date,
            projectId,
            workspaceId);
        using var response = await Client.PutAsJsonAsync(
            $"api/workboard/{workBoardId}",
            request,
            cancellationToken);
        await ThrowIfFailedAsync(response, cancellationToken);
        return await ReadWorkBoardAsync(response, cancellationToken);
    }

    public async Task<int> UpdateStatusAsync(
        Guid workBoardId,
        Guid projectId,
        Guid workspaceId,
        int status,
        CancellationToken cancellationToken = default)
    {
        var request = new WorkBoardStatusUpdateRequest(projectId, workspaceId, status);
        using var response = await Client.PutAsJsonAsync(
            $"api/workboard/{workBoardId}/status",
            request,
            cancellationToken);
        await ThrowIfFailedAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<int>(cancellationToken);
    }

    private HttpClient Client => httpClientFactory.CreateClient("TaskFlowApi");

    private static async Task<WorkBoardResponse> ReadWorkBoardAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken) =>
        await response.Content.ReadFromJsonAsync<WorkBoardResponse>(cancellationToken)
        ?? throw new WorkBoardApiException(500, "The server returned an invalid workboard response.");

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
            throw new WorkBoardApiException(
                (int)response.StatusCode,
                detail ?? problem?.Title ?? "The workboard request could not be completed.");
        }
        catch (WorkBoardApiException)
        {
            throw;
        }
        catch
        {
            throw new WorkBoardApiException(
                (int)response.StatusCode,
                "The workboard request could not be completed.");
        }
    }
}
