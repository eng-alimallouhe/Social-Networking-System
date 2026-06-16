using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Projects.Bridges;

namespace SNS.Infrastructure.Projects.Configurations;

public class SavedProjectConfiguration : IEntityTypeConfiguration<SavedProject>
{
    public void Configure(EntityTypeBuilder<SavedProject> builder)
    {
        builder.ToTable("SavedProjects", "Projects");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ProfileId, x.ProjectId })
               .IsUnique();

        builder.HasIndex(x => new { x.ProfileId, x.SavedAt });

        builder.HasIndex(x => x.ProjectId);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(x => x.ProfileId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.Project)
               .WithMany(p => p.Saves)
               .HasForeignKey(x => x.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
