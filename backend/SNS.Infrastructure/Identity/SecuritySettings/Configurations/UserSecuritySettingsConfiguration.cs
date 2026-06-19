using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Identity.SecuritySettings.Configurations;

public class UserSecuritySettingsConfiguration : IEntityTypeConfiguration<UserSecuritySettings>
{
    public void Configure(EntityTypeBuilder<UserSecuritySettings> builder)
    {
        builder.ToTable("UsersSecuritySettings", "Identity");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RecoveryEmail)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.HasIndex(x => x.RecoveryEmail)
            .IsUnique()
            .HasFilter("[RecoveryEmail] IS NOT NULL");

        builder.Property(x => x.DefaultCommunicationMethod)
            .HasColumnType("int");

        builder.Property(x => x.MfaProvider)
            .HasColumnType("int");


        builder.HasOne<User>()
            .WithOne(u => u.UserSecuritySettings)
            .HasForeignKey<UserSecuritySettings>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
