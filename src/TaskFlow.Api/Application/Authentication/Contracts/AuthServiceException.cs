namespace TaskFlow.src.Application.Authentication.Contracts;

public sealed class AuthServiceException(int statusCode, string title,
    IReadOnlyDictionary<string, string[]>? errors = null) : Exception(title)
{
    public int StatusCode { get; } = statusCode;
    public string Title { get; } = title;
    public IReadOnlyDictionary<string, string[]>? Errors { get; } = errors;
}
