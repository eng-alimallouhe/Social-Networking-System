using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Security.Entities;

namespace SNS.Infrastructure.Configurations.Security;

public class RefreshTokenConfigurations : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);
        builder.HasIndex(rt => rt.UserId);

        builder.Property(rt => rt.Token)
               .IsRequired()
               .HasMaxLength(512)
               .HasColumnType("varchar(512)");

        builder.HasOne<User>()
               .WithMany(u => u.Token)
               .HasForeignKey(rt => rt.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
    }
}