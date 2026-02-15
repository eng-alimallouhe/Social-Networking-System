using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.Preferences;

public class SkillRequestConfigurations :
    IEntityTypeConfiguration<SkillRequest>
{
    public void Configure(EntityTypeBuilder<SkillRequest> builder)
    {
        builder.ToTable("SkillRequests");

        builder.HasKey(sr => sr.Id);
        builder.HasIndex(sr => sr.SubmitterId);

        builder.Property(sr => sr.SkillName)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(sr => sr.Level).HasConversion<int>();
        builder.Property(sr => sr.Status).HasConversion<int>();

        builder.HasOne(sr => sr.Submitter)
               .WithMany()
               .HasForeignKey(sr => sr.SubmitterId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict); 
    }
}