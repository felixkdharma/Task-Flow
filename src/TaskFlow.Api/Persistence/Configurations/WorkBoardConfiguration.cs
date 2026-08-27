using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.Configurations
{
    public sealed class WorkBoardConfiguration : IEntityTypeConfiguration<WorkBoard>
    {
        public void Configure(EntityTypeBuilder<WorkBoard> builder)
        {
            builder.ToTable("Workboards");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.WorkBoardName).HasMaxLength(200);
            builder.Property(x => x.WorkspaceId);
            builder.Property(x => x.ProjectId);
            builder.Property(x => x.UserId);
            builder.Property(x => x.WorkBoardStatus);
            builder.Property(x => x.WorkBoardDescription).HasMaxLength(200);
            builder.Property(x => x.StartDate);
            builder.Property(x => x.EndDate);
        }
    }
}
