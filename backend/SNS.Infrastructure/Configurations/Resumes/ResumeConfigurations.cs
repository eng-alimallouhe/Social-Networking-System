using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Resumes.Entities;

namespace SNS.Infrastructure.Configurations.Resumes;

public class ResumeConfigurations : 
    IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.ToTable("Resumes");

        builder.HasKey(r => r.Id);

        // Foreign Key Index
        builder.HasIndex(r => r.OwnerId);

        // Search Index (Title is often searched)
        builder.HasIndex(r => r.Title);

        builder.Property(r => r.Title)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(r => r.PersonalPictureUrl)
               .HasMaxLength(512)
               .HasColumnType("varchar(512)");

        builder.Property(r => r.Summary)
               .HasMaxLength(2000) // Summaries can be long
               .HasColumnType("nvarchar(2000)");

        // Enums
        builder.Property(r => r.Template)
               .HasConversion<int>();

        builder.Property(r => r.Langauge)
               .HasConversion<int>();

        // Relationship: Profile -> Resumes
        builder.HasOne(r => r.Owner)
               .WithMany(p => p.Resumes)
               .HasForeignKey(r => r.OwnerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}