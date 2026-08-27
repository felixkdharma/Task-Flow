using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.Workspace.Contracts
{
    public sealed record WorkspaceRequest
    (
        [Required, MaxLength(100)] string WorkspaceName,
        string WorkspaceDescription,
        Guid ProjectId,
        Guid UserId
        );
}
