namespace TaskFlow.Application.Workboard.Contracts
{
    public sealed record class WorkBoardResponse
    (
        Guid ProjectId,
        Guid WorkspaceId,
        Guid WorkBoardId,
        DateTime StartDate,
        DateTime EndDate,        
        string WorkBoardName,
        string WorkBoardDescription,
        int WorkBoardStatus
    );
}
