using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Workboard.Contracts;
using TaskFlow.Application.Workboard.Interfaces;
using TaskFlow.src.Persistence;

namespace TaskFlow.Infrastructure.WorkBoard
{
    public sealed class WorkBoardService(ApplicationDbContext dbContext) : IWorkBoardService
    {
        public async Task<List<WorkBoardResponse>> GetAllWorkBoard(
            Guid userId,
            Guid projectId,
            Guid workspaceId,
            CancellationToken cancellationToken)
        {
            return await dbContext.WorkBoards.AsNoTracking()
                .Where(workBoard => workBoard.ProjectId == projectId &&
                    workBoard.WorkspaceId == workspaceId &&
                    workBoard.UserId == userId)
                .OrderBy(workBoard => workBoard.WorkBoardName)
                .Select(workBoard => new WorkBoardResponse(
                    workBoard.ProjectId,
                    workBoard.WorkspaceId,
                    workBoard.Id,
                    workBoard.StartDate,
                    workBoard.EndDate,
                    workBoard.WorkBoardName!,
                    workBoard.WorkBoardDescription!,
                    workBoard.WorkBoardStatus))
                .ToListAsync(cancellationToken);
        }

        public async Task<WorkBoardResponse?> CreateWorkBoard(
            Guid userId,
            WorkboardRequest request,
            CancellationToken cancellationToken)
        {
            var ownsProject = await dbContext.Projects.AnyAsync(
                project => project.Id == request.ProjectId && project.CreatedById == userId,
                cancellationToken);
            var ownsWorkspace = await dbContext.Workspaces.AnyAsync(
                workspace => workspace.Id == request.WorkspaceId &&
                    workspace.ProjectId == request.ProjectId &&
                    workspace.UserId == userId,
                cancellationToken);
            if (!ownsProject || !ownsWorkspace)
            {
                return null;
            }

            var workBoard = new Domain.Entities.WorkBoard
            {
                Id = Guid.NewGuid(),
                ProjectId = request.ProjectId,
                WorkspaceId = request.WorkspaceId,
                WorkBoardName = request.WorkBoardName.Trim(),
                WorkBoardDescription = request.WorkBoardDescription?.Trim() ?? string.Empty,
                StartDate = request.StartDate.Date,
                EndDate = request.EndDate.Date,
                WorkBoardStatus = request.WorkBoardStatus,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.WorkBoards.AddAsync(workBoard, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ToResponse(workBoard);
        }

        public async Task<WorkBoardResponse?> UpdateWorkBoard(
            Guid userId,
            Guid workBoardId,
            WorkBoardDetailsUpdateRequest request,
            CancellationToken cancellationToken)
        {
            var workBoard = await FindOwnedWorkBoardAsync(
                userId,
                workBoardId,
                request.ProjectId,
                request.WorkspaceId,
                cancellationToken);
            if (workBoard is null)
            {
                return null;
            }

            workBoard.WorkBoardName = request.WorkBoardName.Trim();
            workBoard.WorkBoardDescription = request.WorkBoardDescription?.Trim() ?? string.Empty;
            workBoard.StartDate = request.StartDate.Date;
            workBoard.EndDate = request.EndDate.Date;
            workBoard.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
            return ToResponse(workBoard);
        }

        public async Task<int?> UpdateStatusBoard(
            Guid userId,
            Guid workBoardId,
            WorkBoardUpdateStatusRequest request,
            CancellationToken cancellationToken)
        {
            var workBoard = await FindOwnedWorkBoardAsync(
                userId,
                workBoardId,
                request.ProjectId,
                request.WorkspaceId,
                cancellationToken);
            if (workBoard is null)
            {
                return null;
            }

            workBoard.WorkBoardStatus = request.WorkBoardStatus;
            workBoard.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return workBoard.WorkBoardStatus;
        }

        private Task<Domain.Entities.WorkBoard?> FindOwnedWorkBoardAsync(
            Guid userId,
            Guid workBoardId,
            Guid projectId,
            Guid workspaceId,
            CancellationToken cancellationToken)
        {
            return dbContext.WorkBoards.FirstOrDefaultAsync(
                workBoard => workBoard.Id == workBoardId &&
                    workBoard.ProjectId == projectId &&
                    workBoard.WorkspaceId == workspaceId &&
                    workBoard.UserId == userId,
                cancellationToken);
        }

        private static WorkBoardResponse ToResponse(Domain.Entities.WorkBoard workBoard)
        {
            return new WorkBoardResponse(
                workBoard.ProjectId,
                workBoard.WorkspaceId,
                workBoard.Id,
                workBoard.StartDate,
                workBoard.EndDate,
                workBoard.WorkBoardName!,
                workBoard.WorkBoardDescription!,
                workBoard.WorkBoardStatus);
        }
    }
}
