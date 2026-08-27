using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.Project.Contracts
{
    public sealed record ProjectRequest
    (
        [Required, MaxLength(100)] string ProjectName,
        [Required, MaxLength(500)] string Description
    );
}
