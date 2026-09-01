using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Jobs.Entities;

namespace SNS.Infrastructure.Configurations.Jobs;

public class CompanyCreateRequestConfiguration : IEntityTypeConfiguration<CompanyCreateRequest>
{
    public void Configure(EntityTypeBuilder<CompanyCreateRequest> builder)
    {
        builder.ToTable("CompanyCreateRequests", "Jobs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Industry)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.WebsiteUrl)
            .HasMaxLength(500);

        builder.Property(x => x.LogoObjectKey)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedCompanyId);
        builder.Property(x => x.ReviewedByProfileId);
        builder.Property(x => x.ReviewNote).HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ReviewedAt);

        builder.HasOne(x => x.Profile)
            .WithMany(p => p.CompanyCreateRequests)
            .HasForeignKey(x => x.ProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProfileId);
        builder.HasIndex(x => x.Status);
    }
}
