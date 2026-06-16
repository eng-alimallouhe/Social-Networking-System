using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.SecuritySettings.Entities;

namespace SNS.Infrastructure.Identity.SecuritySettings.Configurations;

public class RecoveryCodeConfiguration : IEntityTypeConfiguration<RecoveryCode>
{
    public void Configure(EntityTypeBuilder<RecoveryCode> builder)
    {
        builder.ToTable("RecoveryCodes", "Identity");
        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.CodeHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(rc => rc.CodeHash)
            .IsUnique();

        builder.HasOne<UserSecuritySettings>()
            .WithMany(uss => uss.RecoveryCodes)
            .HasForeignKey(rc => rc.UserSecuritySettingsId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
