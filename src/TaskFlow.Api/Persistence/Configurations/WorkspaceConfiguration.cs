using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.Configurations
{
    public sealed class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
    {
        public void Configure(EntityTypeBuilder<Workspace> builder)
        {
            builder.ToTable("Workspaces");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProjectId);
            builder.Property(x => x.WorkspaceName).IsRequired();
            builder.Property(x => x.WorkspaceDescription);
            builder.Property(x => x.UserId);
        }
    }
}
