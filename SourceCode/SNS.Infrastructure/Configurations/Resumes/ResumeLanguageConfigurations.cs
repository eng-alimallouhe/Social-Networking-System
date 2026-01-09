using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Resumes.Entities;

namespace SNS.Infrastructure.Configurations.Resumes;

public class ResumeLanguageConfigurations :
    IEntityTypeConfiguration<ResumeLanguage>
{
    public void Configure(EntityTypeBuilder<ResumeLanguage> builder)
    {
        builder.ToTable("ResumeLanguages");

        builder.HasKey(rl => rl.Id);
        builder.HasIndex(rl => rl.ResumeId);

        builder.HasIndex(
            rl => new 
            { 
                rl.ResumeId, 
                rl.Language 
            }).IsUnique();

        builder.Property(rl => rl.Language)
               .HasConversion<int>();

        builder.Property(rl => rl.Level)
               .HasConversion<int>();

        builder.HasOne(rl => rl.Resume)
               .WithMany(r => r.Languages)
               .HasForeignKey(rl => rl.ResumeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}