using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Web.Models;

public sealed class WorkspaceFormModel
{
    [Required(ErrorMessage = "Workspace name is required."), MaxLength(100, ErrorMessage = "Workspace name cannot exceed 100 characters.")]
    public string WorkspaceName { get; set; } = string.Empty;

    public string WorkspaceDescription { get; set; } = string.Empty;
}

public sealed record WorkspaceRequest(
    string WorkspaceName,
    string WorkspaceDescription,
    Guid ProjectId,
    Guid UserId);

public sealed record WorkspaceResponse(
    Guid ProjectId,
    Guid WorkspaceId,
    string WorkspaceName,
    string WorkspaceDescription);
