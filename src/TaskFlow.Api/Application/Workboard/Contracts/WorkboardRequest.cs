using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.Workboard.Contracts
{
    public sealed record WorkboardRequest
    (
        [Required, MaxLength(200)] string WorkBoardName,
        string WorkBoardDescription,
        DateTime StartDate,
        DateTime EndDate,
        Guid ProjectId,
        Guid WorkspaceId,
        Guid UserId
    );
}
