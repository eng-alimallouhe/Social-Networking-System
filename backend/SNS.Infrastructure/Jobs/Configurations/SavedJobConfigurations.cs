using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Jobs.Relations;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.Configurations.Jobs;

public class SavedJobConfigurations : IEntityTypeConfiguration<SavedJob>
{
    public void Configure(EntityTypeBuilder<SavedJob> builder)
    {
        builder.ToTable("SavedJobs", "Jobs");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ProfileId, x.JobId })
               .IsUnique();

        builder.HasIndex(x => new { x.ProfileId, x.SavedAt });

        builder.HasIndex(x => x.JobId);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(x => x.ProfileId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.Job)
               .WithMany()
               .HasForeignKey(x => x.JobId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
