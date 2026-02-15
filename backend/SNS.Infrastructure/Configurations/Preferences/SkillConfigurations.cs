using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.Preferences;

public class SkillConfigurations : 
    IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("Skills");

        builder.HasKey(s => s.Id);

        builder.HasIndex(s => s.CategoryId);
        
        builder.HasIndex(s => s.Name)
            .IsUnique();

        builder.Property(s => s.Name)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.HasOne(s => s.Category)
               .WithMany(sc => sc.Skills)
               .HasForeignKey(s => s.CategoryId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}