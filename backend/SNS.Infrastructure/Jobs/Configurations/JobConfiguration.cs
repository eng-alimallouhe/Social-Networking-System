using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Jobs.Entities;

namespace SNS.Infrastructure.Configurations.Jobs;

public class JobConfiguration : 
    IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Jobs", "Jobs");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        
        builder.Property(j => j.CurrencyCode)
            .HasMaxLength(3)
            .IsFixedLength();

        // Accurate decimal mapping for money
        builder.Property(j => j.MinSalary).HasPrecision(18, 2);
        builder.Property(j => j.MaxSalary).HasPrecision(18, 2);

        builder.Property(j => j.Type).HasConversion<int>();
        builder.Property(j => j.SalaryType).HasConversion<int>();

        // Indexes for performance
        builder.HasIndex(j => j.CompanyId);
        builder.HasIndex(j => j.Title);

        // Relationships
        builder.HasOne(j => j.Company)
               .WithMany(c => c.PostedJobs)
               .HasForeignKey(j => j.CompanyId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
