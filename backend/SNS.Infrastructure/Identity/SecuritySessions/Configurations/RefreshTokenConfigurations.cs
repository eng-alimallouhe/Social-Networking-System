using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.SecuritySessions.Entities;

namespace SNS.Infrastructure.Identity.SecuritySessions.Configurations;

public class RefreshTokenConfigurations : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", "Identity");

        builder.HasKey(rt => rt.Id);
        builder.HasIndex(rt => rt.SecuritySessionId);

        builder.Property(rt => rt.Token)
               .IsRequired()
               .HasMaxLength(512)
               .HasColumnType("varchar(512)");

        builder.HasOne<SecuritySession>()
               .WithMany(ss => ss.RefreshTokens)
               .HasForeignKey(rt => rt.SecuritySessionId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
    }
}
