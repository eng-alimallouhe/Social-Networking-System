using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Identity.ArchiveManagement.Configurations;

public class PasswordArchiveConfigurations : 
    IEntityTypeConfiguration<PasswordArchive>
{
    public void Configure(EntityTypeBuilder<PasswordArchive> builder)
    {
        builder.ToTable("PasswordArchives", "Identity");

        builder.HasKey(pa => pa.Id);
        builder.HasIndex(pa => pa.UserId);

        builder.HasOne<User>()
               .WithMany(u => u.PasswordArchives)
               .HasForeignKey(pa => pa.UserId)
               .IsRequired();
    }
}
