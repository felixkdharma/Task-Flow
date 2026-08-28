using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.Workboard.Contracts
{
    public sealed record WorkBoardDetailsUpdateRequest
    (
        [Required, MaxLength(200)] string WorkBoardName,
        [MaxLength(200)] string? WorkBoardDescription,
        DateTime StartDate,
        DateTime EndDate,
        Guid ProjectId,
        Guid WorkspaceId
    );
}
