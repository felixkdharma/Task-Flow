using TaskFlow.src.Domain.Common;

namespace TaskFlow.Domain.Entities
{
    public sealed class WorkBoard : BaseEntity
    {
        public Guid WorkspaceId { get; set;  }
        public Guid ProjectId { get; set;  }
        public Guid UserId { get; set;  }
        public string? WorkBoardName { get; set; }
        public string? WorkBoardDescription { get; set; }
        public int WorkBoardStatus;
        public DateTime StartDate { get; set;  }
        public DateTime EndDate { get; set;  }
    }
}
