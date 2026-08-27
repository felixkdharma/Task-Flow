namespace TaskFlow.Application.Project.Contracts
{
    public sealed record class ProjectResponse
    (
        Guid Id,
        string ProjectName,
        string Description
    );
}
