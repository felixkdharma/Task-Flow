using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Workboard.Contracts;
using TaskFlow.Application.Workboard.Interfaces;
using TaskFlow.src.Persistence;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskFlow.Infrastructure.WorkBoard
{
    public sealed class WorkBoardService(ApplicationDbContext dbContext) : IWorkBoardService
    {
        public async Task<List<WorkBoardResponse>> GetAllWorkBoard(Guid userId, Guid projectId, Guid workspaceId, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(userId.ToString()))
            {

                var data = await dbContext.WorkBoards.AsNoTracking()
                    .Where(wb => wb.ProjectId == projectId && wb.WorkspaceId == workspaceId && wb.UserId == userId)
                    .OrderBy(wb => wb.WorkBoardName)
                    .Select(wb => new WorkBoardResponse(
                            wb.ProjectId,
                            wb.WorkspaceId,
                            wb.Id,
                            wb.StartDate,
                            wb.EndDate,
                            wb.WorkBoardName!,
                            wb.WorkBoardDescription!,
                            wb.WorkBoardStatus
                    )).ToListAsync();

                return data;
            }
            else { 
                throw new NotImplementedException();
            }
        }
    }
}
