namespace TaskFlow.Application.Workspace.Contracts
{
    public sealed record class WorkspaceResponse
    (
        Guid ProjectId,
        Guid WorkspaceId,
        string WorkspaceName,
        string WorkspaceDescription
        );
}
