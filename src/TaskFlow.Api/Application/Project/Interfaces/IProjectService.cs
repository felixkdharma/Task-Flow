using TaskFlow.Application.Project.Contracts;

namespace TaskFlow.Application.Project.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectResponse>> GetProjectsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ProjectResponse> CreateProjectAsync(Guid userId, ProjectRequest request, CancellationToken cancellationToken = default);
        Task<ProjectResponse?> UpdateProjectAsync(Guid userId, Guid id, ProjectRequest request, CancellationToken cancellationToken = default);
        Task<ProjectResponse?> GetDetailProjectByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
        Task<bool> DeleteProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
    }
}
