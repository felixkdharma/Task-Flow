using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Project.Contracts;
using TaskFlow.Application.Project.Interfaces;
using TaskFlow.src.Persistence;

namespace TaskFlow.Infrastructure.Project;

public sealed class ProjectService(ApplicationDbContext dbContext) : IProjectService
{
    public async Task<ProjectResponse> CreateProjectAsync(
        Guid userId,
        ProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        var project = new Domain.Entities.Project
        {
            Id = Guid.NewGuid(),
            CreatedById = userId,
            ProjectName = request.ProjectName.Trim(),
            Description = request.Description.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        await dbContext.Projects.AddAsync(project, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(project);
    }

    public Task<List<ProjectResponse>> GetProjectsAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        dbContext.Projects
            .AsNoTracking()
            .Where(project => project.CreatedById == userId)
            .OrderBy(project => project.ProjectName)
            .Select(project => new ProjectResponse(
                project.Id,
                project.ProjectName!,
                project.Description!))
            .ToListAsync(cancellationToken);

    public Task<ProjectResponse?> GetDetailProjectByIdAsync(
        Guid userId,
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Projects
            .AsNoTracking()
            .Where(project => project.Id == id && project.CreatedById == userId)
            .Select(project => new ProjectResponse(
                project.Id,
                project.ProjectName!,
                project.Description!))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ProjectResponse?> UpdateProjectAsync(
        Guid userId,
        Guid id,
        ProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects
            .FirstOrDefaultAsync(
                project => project.Id == id && project.CreatedById == userId,
                cancellationToken);
        if (project is null)
        {
            return null;
        }

        project.ProjectName = request.ProjectName.Trim();
        project.Description = request.Description.Trim();
        project.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(project);
    }

    public async Task<bool> DeleteProjectAsync(
        Guid userId,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects
            .FirstOrDefaultAsync(
                project => project.Id == projectId && project.CreatedById == userId,
                cancellationToken);
        if (project is null)
        {
            return false;
        }

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ProjectResponse ToResponse(Domain.Entities.Project project) =>
        new(project.Id, project.ProjectName!, project.Description!);
}
