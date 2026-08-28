using TaskFlow.Application.Workboard.Contracts;

namespace TaskFlow.Application.Workboard.Interfaces
{
    public interface IWorkBoardService
    {
        Task<List<WorkBoardResponse>> GetAllWorkBoard(Guid userId, Guid projectId, Guid workspaceId, CancellationToken cancellationToken);
        Task<WorkBoardResponse?> CreateWorkBoard(Guid userId, WorkboardRequest request, CancellationToken cancellationToken);
        Task<WorkBoardResponse?> UpdateWorkBoard(Guid userId, Guid workBoardId, WorkBoardDetailsUpdateRequest request, CancellationToken cancellationToken);
        Task<int?> UpdateStatusBoard(Guid userId, Guid workBoardId, WorkBoardUpdateStatusRequest request, CancellationToken cancellationToken);
    }
}
