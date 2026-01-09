using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.Preferences;

public class TopicConfigurations : 
    IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable("Topics");

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.Name).IsUnique();

        builder.Property(t => t.Name)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");
    }
}