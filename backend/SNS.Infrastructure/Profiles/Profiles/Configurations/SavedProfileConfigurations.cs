using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.Profiles.Profiles.Configurations;

public class SavedProfileConfigurations : IEntityTypeConfiguration<SavedProfile>
{
    public void Configure(EntityTypeBuilder<SavedProfile> builder)
    {
        builder.ToTable("SavedProfiles", "Profiles");

        builder.HasKey(sp => sp.Id);
        
        builder.HasIndex(sp => new { sp.SaverId, sp.SavedId }).IsUnique();
        builder.HasIndex(sp => sp.SavedId);

        builder.HasOne(sp => sp.Saver)
            .WithMany(p => p.SavedProfiles)
            .HasForeignKey(sp => sp.SaverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.Saved)
            .WithMany(p => p.SavedByProfiles)
            .HasForeignKey(sp => sp.SavedId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
