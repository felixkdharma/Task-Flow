using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Application.Workboard.Contracts;
using TaskFlow.Application.Workboard.Interfaces;

namespace TaskFlow.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/workboard")]
    public sealed class WorkBoardController(IWorkBoardService workBoardService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<WorkBoardResponse>>> GetAllWorkBoard(Guid projectId, Guid workspaceId, CancellationToken cancellationToken) {

            if (!TryGetUserId(out var userId)) {

                return Unauthorized();
            
            }

            var data = await workBoardService.GetAllWorkBoard(userId, projectId, workspaceId, cancellationToken);

            return Ok(data);

        }

        private bool TryGetUserId(out Guid userId) =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}
