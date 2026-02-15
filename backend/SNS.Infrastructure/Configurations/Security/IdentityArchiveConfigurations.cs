using SNS.Domain.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SNS.Infrastructure.Configurations.Security;

public class IdentityArchiveConfigurations : 
    IEntityTypeConfiguration<IdentityArchive>
{
    public void Configure(EntityTypeBuilder<IdentityArchive> builder)
    {
        builder.ToTable("IdentityArchives");

        builder.HasKey(ia => ia.Id);
        builder.HasIndex(ia => ia.UserId);

        builder.Property(ia => ia.UserIdentifier)
               .IsRequired()
               .HasMaxLength(255)
               .HasColumnType("nvarchar(255)");

        builder.HasOne(ia => ia.User)
               .WithMany(u => u.IdentityArchives)
               .HasForeignKey(ia => ia.UserId)
               .IsRequired();
    }
}