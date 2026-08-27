using TaskFlow.src.Domain.Common;

namespace TaskFlow.Domain.Entities
{
    public sealed class Workspace : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string? WorkspaceName { get; set;  }
        public string? WorkspaceDescription { get; set; }
    }
}
