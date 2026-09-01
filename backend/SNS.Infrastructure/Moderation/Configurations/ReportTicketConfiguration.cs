using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Moderation.Entities;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Moderation.Configurations;

public class ReportTicketConfiguration : IEntityTypeConfiguration<ReportTicket>
{
    public void Configure(EntityTypeBuilder<ReportTicket> builder)
    {
        builder.ToTable("ReportTickets", "Moderation");

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.TargetId);

        builder.Property(t => t.TargetType).HasConversion<int>();
        builder.Property(t => t.Status).HasConversion<int>();

        builder.Property(t => t.ModeratorNotes)
               .HasMaxLength(1000)
               .HasColumnType("nvarchar(1000)");

        // Relationships
        builder.HasMany(t => t.Reports)
               .WithOne()
               .HasForeignKey(r => r.TicketId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Moderator)
               .WithMany()
               .HasForeignKey(t => t.ModeratorId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
