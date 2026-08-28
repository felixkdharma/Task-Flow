namespace TaskFlow.Web.WorkBoards;

public sealed class WorkBoardApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
