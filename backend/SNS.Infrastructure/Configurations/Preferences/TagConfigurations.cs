using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.Preferences;

public class TagConfigurations : 
    IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.Name).IsUnique();

        builder.Property(t => t.Name)
               .IsRequired()
               .HasMaxLength(50)
               .HasColumnType("nvarchar(50)");
    }
}