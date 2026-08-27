using TaskFlow.src.Domain.Common;

namespace TaskFlow.Domain.Entities
{
    public sealed class Project : BaseEntity
    {
        public Guid CreatedById { get; set; }
        public string? ProjectName { get; set;  }
        public string? Description { get; set; }
    }
}
