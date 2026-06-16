using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Educations.Entities;

namespace SNS.Infrastructure.Education.Configurations;

public class UniversityConfiguration :
    IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.ToTable("Universities", "Education");

        builder.HasKey(u => u.Id);

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
