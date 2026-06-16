using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Preferences.Configurations;

public class SkillsCategoryConfigurations :
    IEntityTypeConfiguration<SkillsCategory>
{
    public void Configure(EntityTypeBuilder<SkillsCategory> builder)
    {
        builder.ToTable("SkillsCategories", "Preferences");

        builder.HasKey(sc => sc.Id);

        builder.HasIndex(sc => sc.Name).IsUnique();

        builder.Property(sc => sc.Name)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(sc => sc.Description)
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");
    }
}
