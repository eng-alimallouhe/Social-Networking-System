using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.Preferences;

public class InterestConfigurations :
    IEntityTypeConfiguration<Interest>
{
    public void Configure(EntityTypeBuilder<Interest> builder)
    {
        builder.ToTable("Interests");

        builder.HasKey(i => i.Id);

        builder.HasIndex(i => i.Name)
            .IsUnique(); // Unique Name

        builder.Property(i => i.Name)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(i => i.Description)
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");

        builder.HasOne(i => i.Category)
               .WithMany(ic => ic.Interests)
               .HasForeignKey(i => i.CategoryId)
               .IsRequired() 
               .OnDelete(DeleteBehavior.Restrict);
    }
}