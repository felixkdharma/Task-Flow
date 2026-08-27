using TaskFlow.Application.Workboard.Contracts;

namespace TaskFlow.Application.Workboard.Interfaces
{
    public interface IWorkBoardService
    {
        Task<List<WorkBoardResponse>> GetAllWorkBoard(Guid userId, Guid projectId, Guid workspaceId, CancellationToken cancellationToken);
    }
}
