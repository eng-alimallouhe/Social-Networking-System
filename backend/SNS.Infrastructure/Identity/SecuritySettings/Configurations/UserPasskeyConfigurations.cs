using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Identity.SecuritySettings.Configurations;

public class UserPasskeyConfigurations : IEntityTypeConfiguration<UserPasskey>
{
    public void Configure(EntityTypeBuilder<UserPasskey> builder)
    {
        builder.ToTable("UserPassKeys", "Identity");

        builder.HasKey(x => x.Id);

        builder.HasOne<User>()
            .WithMany(u => u.Passkeys)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}
