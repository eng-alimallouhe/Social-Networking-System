using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.ArchiveManagement.Entities;

namespace SNS.Infrastructure.Identity.ArchiveManagement.Configurations;

public class IdentityArchiveConfigurations : 
    IEntityTypeConfiguration<IdentityArchive>
{
    public void Configure(EntityTypeBuilder<IdentityArchive> builder)
    {
        builder.ToTable("IdentityArchives", "Identity");

        builder.HasKey(ia => ia.Id);
        builder.HasIndex(ia => ia.UserId);

        builder.Property(ia => ia.OldUserIdentifier)
               .IsRequired()
               .HasMaxLength(255)
               .HasColumnType("nvarchar(255)");
        
        builder.Property(ia => ia.NewUserIdentifier)
               .IsRequired()
               .HasMaxLength(255)
               .HasColumnType("nvarchar(255)");

        builder.HasOne<User>()
               .WithMany(u => u.IdentityArchives)
               .HasForeignKey(ia => ia.UserId)
               .IsRequired();
    }
}
