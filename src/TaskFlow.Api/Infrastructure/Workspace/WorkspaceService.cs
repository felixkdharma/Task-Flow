using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Workspace.Contracts;
using TaskFlow.Application.Workspace.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.src.Persistence;

namespace TaskFlow.Infrastructure.Workspace
{
    public sealed class WorkspaceService(ApplicationDbContext dbContext) : IWorkspaceService
    {
        public async Task<WorkspaceResponse> CreateWorkspace(WorkspaceRequest workspaceRequest, CancellationToken cancellationToken)
        {
            var exist = await dbContext.Workspaces.Where(x => x.ProjectId == workspaceRequest.ProjectId ).FirstOrDefaultAsync();

            if (exist == null)
            {

                var toAdd = new Domain.Entities.Workspace
                {
                    Id = Guid.NewGuid(),
                    ProjectId = workspaceRequest.ProjectId,
                    WorkspaceName = workspaceRequest.WorkspaceName,
                    WorkspaceDescription = workspaceRequest.WorkspaceDescription,
                    UserId = workspaceRequest.UserId,
                    CreatedAt = DateTime.Now
                };

                var result = new WorkspaceResponse(toAdd.ProjectId, toAdd.Id, toAdd.WorkspaceName, toAdd.WorkspaceDescription);

                await dbContext.Workspaces.AddRangeAsync(toAdd);

                await dbContext.SaveChangesAsync(cancellationToken);

                return result;

            }
            else {

                throw new Exception(" Invalid Project Id ");

            }

        }

        public async Task<bool> DeleteWorkspace(Guid projectId, Guid workspaceId, CancellationToken cancellationToken)
        {
            var exist = await dbContext.Workspaces.AsNoTracking().Where(x => x.ProjectId == projectId && x.Id == workspaceId).FirstOrDefaultAsync();

            if (exist != null)
            {
                dbContext.RemoveRange(exist);

                await dbContext.SaveChangesAsync(cancellationToken);

                return true;
            }
            else {

                return false;

                throw new Exception(" Invalid Workspace Id ");
            }

        }

        public async Task<List<WorkspaceResponse>> GetAllWorkspace(Guid projectId, Guid userId, CancellationToken cancellationToken)
        {

            var data = await dbContext.Workspaces.AsNoTracking().
                Where(x => x.ProjectId == projectId && x.UserId == userId).
                OrderBy(x => x.WorkspaceName).
                Select(x => new WorkspaceResponse ( 
                    x.ProjectId,
                    x.Id,
                    x.WorkspaceName!,
                    x.WorkspaceDescription!  
                )).
                ToListAsync(cancellationToken);

            return data;
            
        }

        public async Task<WorkspaceResponse> UpdateWorkspace(Guid workspaceId, WorkspaceRequest workspaceRequest, CancellationToken cancellationToken)
        {
            var exist = await dbContext.Workspaces.Where(x => x.ProjectId == workspaceRequest.ProjectId && x.Id == workspaceId).FirstOrDefaultAsync();

            if (exist != null)
            {
                exist.WorkspaceName = workspaceRequest.WorkspaceName;
                exist.WorkspaceDescription = workspaceRequest.WorkspaceDescription;
                exist.UpdatedAt = DateTime.Now;

                var result = new WorkspaceResponse(exist.ProjectId, exist.Id, exist.WorkspaceName, exist.WorkspaceDescription);

                await dbContext.SaveChangesAsync(cancellationToken);

                return result;
            }
            else {

                throw new Exception("Invalid WorkspaceId");
            }

        }

        public async Task<WorkspaceResponse> GetWorkspaceById(WorkspaceRequestById request, CancellationToken cancellationToken)
        {
            
            var exist = await dbContext.Workspaces.AsNoTracking().
                Where(x => x.ProjectId == request.ProjectId && x.Id == request.WorkspaceId).FirstOrDefaultAsync(cancellationToken);

            if (exist == null)
            {

                throw new Exception("Invalid Workspace Id");

            }
            else {

                var result = new WorkspaceResponse(
                    exist.ProjectId,
                    exist.Id,
                    exist.WorkspaceName!,
                    exist.WorkspaceDescription!
                    );

                return result;

            }

        }
    }
}
