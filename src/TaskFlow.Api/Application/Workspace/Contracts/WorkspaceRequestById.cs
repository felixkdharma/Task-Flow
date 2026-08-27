namespace TaskFlow.Application.Workspace.Contracts
{
    public sealed record WorkspaceRequestById
    (
        Guid ProjectId,
        Guid WorkspaceId
    );
}
