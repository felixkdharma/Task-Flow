using TaskFlow.Application.Workspace.Contracts;

namespace TaskFlow.Application.Workspace.Interfaces
{
    public interface IWorkspaceService
    {
        Task<List<WorkspaceResponse>> GetAllWorkspace(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
        Task<WorkspaceResponse> GetWorkspaceById (WorkspaceRequestById request, CancellationToken cancellationToken = default);
        Task<WorkspaceResponse> CreateWorkspace (WorkspaceRequest workspaceRequest, CancellationToken cancellationToken = default);
        Task<WorkspaceResponse> UpdateWorkspace (Guid workspaceId, WorkspaceRequest workspaceRequest, CancellationToken cancellationToken);
        Task<bool> DeleteWorkspace (Guid projectId, Guid workspaceId, CancellationToken cancellationToken);
    }
}
