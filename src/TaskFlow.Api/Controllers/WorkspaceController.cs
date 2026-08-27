using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Application.Workspace.Contracts;
using TaskFlow.Application.Workspace.Interfaces;

namespace TaskFlow.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/workspace")]
    public sealed class WorkspaceController(IWorkspaceService workspaceService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<WorkspaceResponse>>> GetAllWorkspace(Guid projectId, CancellationToken cancellationToken)
        {

            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }
                
            return Ok(await workspaceService.GetAllWorkspace(projectId, userId, cancellationToken));

        }

        [HttpPost]
        public async Task<ActionResult<WorkspaceResponse>> CreateWorkspace(WorkspaceRequest request, CancellationToken cancellationToken) {

            if (!TryGetUserId(out var userId)) {
                return Unauthorized();
            }

            var response = await workspaceService.CreateWorkspace(request, cancellationToken);

            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<WorkspaceResponse>> UpdateWorkspace(Guid workspaceId, WorkspaceRequest request, CancellationToken cancellationToken) {

            if (!TryGetUserId(out var userId)) {
                return Unauthorized();
            }

            var response = await workspaceService.UpdateWorkspace(workspaceId, request, cancellationToken);

            return response is null ? NotFound() : Ok(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteWorkspace(Guid projectId, Guid workspaceId, CancellationToken cancellationToken) {

            if (!TryGetUserId(out var userId)) {
                return Unauthorized();
            }

            return await workspaceService.DeleteWorkspace(projectId, workspaceId, cancellationToken) ? NoContent() : NotFound();
        }

        private bool TryGetUserId(out Guid userId) =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}
