using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.Preferences;

public class InterestCategoryConfigurations :
    IEntityTypeConfiguration<InterestCategory>
{
    public void Configure(EntityTypeBuilder<InterestCategory> builder)
    {
        builder.ToTable("InterestCategories");

        builder.HasKey(ic => ic.Id);

        builder.HasIndex(ic => ic.Name)
            .IsUnique(); 

        builder.Property(ic => ic.Name)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(ic => ic.Description)
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");
    }
}