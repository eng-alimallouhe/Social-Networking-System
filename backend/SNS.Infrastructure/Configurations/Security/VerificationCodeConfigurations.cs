using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Security.Entities;

namespace SNS.Infrastructure.Configurations.Security;

public class VerificationCodeConfigurations :
    IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.ToTable("VerificationCodes");

        builder.HasKey(vc => vc.Id);
        builder.HasIndex(vc => vc.UserId);
        builder.HasIndex(vc => vc.PendingUpdateId);

        builder.Property(vc => vc.Code)
               .IsRequired()
               .HasMaxLength(10)
               .HasColumnType("varchar(10)");

        builder.HasOne(vc => vc.User)
               .WithMany(u => u.VerificationCodes)
               .HasForeignKey(vc => vc.UserId)
               .IsRequired();

        builder.HasOne(vc => vc.PendingUpdate)
               .WithOne(pu => pu.VerificationCode)
               .HasForeignKey<VerificationCode>(vc => vc.PendingUpdateId)
               .IsRequired(false);
    }
}