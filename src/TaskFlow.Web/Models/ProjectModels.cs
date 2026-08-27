using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Web.Models;

public sealed class ProjectFormModel
{
    [Required(ErrorMessage = "Project name is required."), MaxLength(100, ErrorMessage = "Project name cannot exceed 100 characters.")]
    public string ProjectName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required."), MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; } = string.Empty;
}

public sealed record ProjectRequest(string ProjectName, string Description);
public sealed record ProjectResponse(Guid Id, string ProjectName, string Description);
