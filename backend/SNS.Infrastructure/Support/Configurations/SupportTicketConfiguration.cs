using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Support.Entities;

namespace SNS.Infrastructure.Support.Configurations;

public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("SupportTickets", "Support");

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.AssignedAgentId);

        builder.Property(t => t.Title)
               .IsRequired()
               .HasMaxLength(200)
               .HasColumnType("nvarchar(200)");

        builder.Property(t => t.Category).HasConversion<int>();
        builder.Property(t => t.Priority).HasConversion<int>();
        builder.Property(t => t.Status).HasConversion<int>();

        // Relationships
        builder.HasMany(t => t.Messages)
               .WithOne()
               .HasForeignKey(m => m.TicketId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
