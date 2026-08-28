using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Web.Models;

public sealed class WorkBoardFormModel : IValidatableObject
{
    [Required(ErrorMessage = "Workboard name is required."), MaxLength(200, ErrorMessage = "Workboard name cannot exceed 200 characters.")]
    public string WorkBoardName { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
    public string WorkBoardDescription { get; set; } = string.Empty;

    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime EndDate { get; set; } = DateTime.Today;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate == default)
        {
            yield return new ValidationResult("Start date is required.", [nameof(StartDate)]);
        }
        if (EndDate == default)
        {
            yield return new ValidationResult("End date is required.", [nameof(EndDate)]);
        }
        if (StartDate != default && EndDate != default && EndDate.Date < StartDate.Date)
        {
            yield return new ValidationResult("End date cannot be earlier than start date.", [nameof(EndDate)]);
        }
    }
}

public sealed record WorkBoardCreateRequest(
    string WorkBoardName,
    string WorkBoardDescription,
    int WorkBoardStatus,
    DateTime StartDate,
    DateTime EndDate,
    Guid ProjectId,
    Guid WorkspaceId);

public sealed record WorkBoardDetailsUpdateRequest(
    string WorkBoardName,
    string WorkBoardDescription,
    DateTime StartDate,
    DateTime EndDate,
    Guid ProjectId,
    Guid WorkspaceId);

public sealed record WorkBoardStatusUpdateRequest(
    Guid ProjectId,
    Guid WorkspaceId,
    int WorkBoardStatus);

public sealed record WorkBoardResponse(
    Guid ProjectId,
    Guid WorkspaceId,
    Guid WorkBoardId,
    DateTime StartDate,
    DateTime EndDate,
    string WorkBoardName,
    string WorkBoardDescription,
    int WorkBoardStatus);
