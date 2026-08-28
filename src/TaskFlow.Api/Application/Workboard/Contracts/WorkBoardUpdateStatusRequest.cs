using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.Workboard.Contracts
{
    public sealed record WorkBoardUpdateStatusRequest
    (
        Guid ProjectId,
        Guid WorkspaceId,
        int WorkBoardStatus
    );
}
