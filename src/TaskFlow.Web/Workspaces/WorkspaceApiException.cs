namespace TaskFlow.Web.Workspaces;

public sealed class WorkspaceApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
