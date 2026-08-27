using System.Net;
using System.Net.Http.Json;
using TaskFlow.Web.Models;

namespace TaskFlow.Web.Projects;

public sealed class ProjectApiService(IHttpClientFactory httpClientFactory)
{
    public async Task<IReadOnlyList<ProjectResponse>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var response = await Client.GetAsync("api/project", cancellationToken);
        await ThrowIfFailedAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<List<ProjectResponse>>(cancellationToken)
            ?? [];
    }

    public async Task<ProjectResponse?> GetProjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await Client.GetAsync($"api/project/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ThrowIfFailedAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ProjectResponse>(cancellationToken)
            ?? throw new ProjectApiException(500, "The server returned an invalid project response.");
    }

    public async Task<ProjectResponse> CreateProjectAsync(
        ProjectFormModel model,
        CancellationToken cancellationToken = default)
    {
        var request = new ProjectRequest(model.ProjectName.Trim(), model.Description.Trim());
        var response = await Client.PostAsJsonAsync("api/project", request, cancellationToken);
        await ThrowIfFailedAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ProjectResponse>(cancellationToken)
            ?? throw new ProjectApiException(500, "The server returned an invalid project response.");
    }

    private HttpClient Client => httpClientFactory.CreateClient("TaskFlowApi");

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
            throw new ProjectApiException(
                (int)response.StatusCode,
                detail ?? problem?.Title ?? "The project request could not be completed.");
        }
        catch (ProjectApiException)
        {
            throw;
        }
        catch
        {
            throw new ProjectApiException(
                (int)response.StatusCode,
                "The project request could not be completed.");
        }
    }
}
