using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Jobs.Entities;

namespace SNS.Infrastructure.Configurations.Jobs;

public class CompanyAdministratorConfigurations : IEntityTypeConfiguration<CompanyAdministrator>
{
    public void Configure(EntityTypeBuilder<CompanyAdministrator> builder)
    {
        builder.ToTable("CompanyAdministrators", "Jobs");

        builder.HasKey(ca => ca.Id);

        builder.HasIndex(ca => new 
        {
            ca.ProfileId,
            ca.CompanyId
        }).IsUnique();


        builder.HasOne(ca => ca.Profile)
            .WithMany()
            .HasForeignKey(ca => ca.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    
        builder.HasOne(ca => ca.Company)
            .WithMany(c => c.Administrators)
            .HasForeignKey(ca => ca.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
