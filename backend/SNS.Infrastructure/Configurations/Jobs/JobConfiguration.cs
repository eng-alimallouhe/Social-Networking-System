using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Jobs.Entities;

namespace SNS.Infrastructure.Configurations.Jobs;

public class JobConfiguration : 
    IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Jobs");

        builder.HasKey(j => j.Id);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(j => j.IsActive);

        builder.Property(j => j.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(j => j.Company)
            .IsRequired()
            .HasMaxLength(150);
        
        builder.Property(j => j.CurrencyCode)
            .HasMaxLength(3)
            .IsFixedLength();

        // Accurate decimal mapping for money
        builder.Property(j => j.MinSalary).HasPrecision(18, 2);
        builder.Property(j => j.MaxSalary).HasPrecision(18, 2);

        builder.Property(j => j.Type).HasConversion<int>();
        builder.Property(j => j.SalaryType).HasConversion<int>();

        // Indexes for performance
        builder.HasIndex(j => j.OwnerId);
        builder.HasIndex(j => j.Title);

        // Relationships
        builder.HasOne(j => j.Owner)
               .WithMany() // Assuming Profile doesn't have a Jobs collection
               .HasForeignKey(j => j.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}