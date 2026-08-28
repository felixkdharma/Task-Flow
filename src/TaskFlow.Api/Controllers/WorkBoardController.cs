using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Application.Workboard.Contracts;
using TaskFlow.Application.Workboard.Interfaces;
using TaskFlow.Domain.Common;

namespace TaskFlow.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/workboard")]
    public sealed class WorkBoardController(IWorkBoardService workBoardService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<WorkBoardResponse>>> GetAllWorkBoard(
            Guid projectId,
            Guid workspaceId,
            CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var data = await workBoardService.GetAllWorkBoard(userId, projectId, workspaceId, cancellationToken);
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult<WorkBoardResponse>> CreateWorkBoard(
            WorkboardRequest request,
            CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var requestIsValid = ValidateRequest(request.ProjectId, request.WorkspaceId, request.StartDate, request.EndDate);
            var statusIsValid = ValidateStatus(request.WorkBoardStatus);
            if (!requestIsValid || !statusIsValid)
            {
                return ValidationProblem(ModelState);
            }

            var response = await workBoardService.CreateWorkBoard(userId, request, cancellationToken);
            return response is null ? NotFound() : Ok(response);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<WorkBoardResponse>> UpdateWorkBoard(
            Guid id,
            WorkBoardDetailsUpdateRequest request,
            CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            if (!ValidateRequest(request.ProjectId, request.WorkspaceId, request.StartDate, request.EndDate))
            {
                return ValidationProblem(ModelState);
            }

            var response = await workBoardService.UpdateWorkBoard(userId, id, request, cancellationToken);
            return response is null ? NotFound() : Ok(response);
        }

        [HttpPut("{id:guid}/status")]
        public async Task<ActionResult<int>> UpdateStatusWorkBoard(
            Guid id,
            WorkBoardUpdateStatusRequest request,
            CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var idsAreValid = ValidateIds(request.ProjectId, request.WorkspaceId);
            var statusIsValid = ValidateStatus(request.WorkBoardStatus);
            if (!idsAreValid || !statusIsValid)
            {
                return ValidationProblem(ModelState);
            }

            var response = await workBoardService.UpdateStatusBoard(userId, id, request, cancellationToken);
            return response is null ? NotFound() : Ok(response.Value);
        }

        private bool ValidateRequest(Guid projectId, Guid workspaceId, DateTime startDate, DateTime endDate)
        {
            ValidateIds(projectId, workspaceId);
            if (startDate == default)
            {
                ModelState.AddModelError(nameof(startDate), "Start date is required.");
            }
            if (endDate == default)
            {
                ModelState.AddModelError(nameof(endDate), "End date is required.");
            }
            if (startDate != default && endDate != default && endDate.Date < startDate.Date)
            {
                ModelState.AddModelError(nameof(endDate), "End date cannot be earlier than start date.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateIds(Guid projectId, Guid workspaceId)
        {
            if (projectId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(projectId), "Project is required.");
            }
            if (workspaceId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(workspaceId), "Workspace is required.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateStatus(int status)
        {
            if (Enum.IsDefined(typeof(EnumWorkBoard), status))
            {
                return true;
            }

            ModelState.AddModelError(nameof(status), "Workboard status must be Backlog (99), To do (1), In progress (2), or Complete (100).");
            return false;
        }

        private bool TryGetUserId(out Guid userId) =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}
