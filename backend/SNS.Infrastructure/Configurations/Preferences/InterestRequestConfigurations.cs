using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.Preferences;

public class InterestRequestConfigurations :
    IEntityTypeConfiguration<InterestRequest>
{
    public void Configure(EntityTypeBuilder<InterestRequest> builder)
    {
        builder.ToTable("InterestRequests");

        builder.HasKey(ir => ir.Id);

        builder.HasIndex(ir => ir.SubmitterId);

        builder.Property(ir => ir.Name)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(ir => ir.Description)
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");

        builder.Property(ir => ir.Status)
               .HasConversion<int>();

        builder.HasOne(ir => ir.Submitter)
               .WithMany()
               .HasForeignKey(ir => ir.SubmitterId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}