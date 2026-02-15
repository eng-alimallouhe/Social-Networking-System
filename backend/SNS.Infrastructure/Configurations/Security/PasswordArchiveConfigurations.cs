using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Security.Entities;

namespace SNS.Infrastructure.Configurations.Security;

public class PasswordArchiveConfigurations : 
    IEntityTypeConfiguration<PasswordArchive>
{
    public void Configure(EntityTypeBuilder<PasswordArchive> builder)
    {
        builder.ToTable("PasswordArchives");

        builder.HasKey(pa => pa.Id);
        builder.HasIndex(pa => pa.UserId);

        builder.Property(pa => pa.HashedPassword)
               .IsRequired()
               .HasMaxLength(512)
               .HasColumnType("nvarchar(512)");

        builder.HasOne(pa => pa.User)
               .WithMany(u => u.PasswordArchives)
               .HasForeignKey(pa => pa.UserId)
               .IsRequired();
    }
}