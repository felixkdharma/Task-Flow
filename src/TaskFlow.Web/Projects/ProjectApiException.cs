namespace TaskFlow.Web.Projects;

public sealed class ProjectApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
