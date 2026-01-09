using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Education.Entities;

namespace SNS.Infrastructure.Configurations.Education;

public class UniversityConfiguration :
    IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.ToTable("Universities");

        builder.HasKey(u => u.Id);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(u => u.IsActive);

        builder.HasIndex(u => u.Name); // Search Index

        builder.Property(u => u.Name)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(u => u.Country)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(u => u.City)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");
    }
}