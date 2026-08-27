namespace TaskFlow.Web.Authentication;

public sealed class AuthApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
