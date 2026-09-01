using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Moderation.Entities;

namespace SNS.Infrastructure.Moderation.Configurations;

public class ContentReportConfiguration : IEntityTypeConfiguration<ContentReport>
{
    public void Configure(EntityTypeBuilder<ContentReport> builder)
    {
        builder.ToTable("ContentReports", "Moderation");

        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.TicketId);
        builder.HasIndex(c => c.ReporterId);

        builder.Property(c => c.ViolationReason).HasConversion<int>();

        builder.Property(c => c.AdditionalDetails)
               .HasMaxLength(1000)
               .HasColumnType("nvarchar(1000)");
    }
}
