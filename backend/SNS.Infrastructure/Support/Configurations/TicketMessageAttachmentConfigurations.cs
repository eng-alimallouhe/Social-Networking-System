using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Support.Entities;

namespace SNS.Infrastructure.Support.Configurations;

public class TicketMessageAttachmentConfigurations : IEntityTypeConfiguration<TicketMessageAttachment>
{
    public void Configure(EntityTypeBuilder<TicketMessageAttachment> builder)
    {
        builder.ToTable("TicketMessageAttachments", "Support");

        builder.HasKey(a => a.Id);

        builder.HasIndex(a => a.TicketMessageId);

        builder.Property(a => a.ObjectKey)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");

        builder.HasOne<TicketMessage>()
               .WithMany(m => m.Attachments)
               .HasForeignKey(a => a.TicketMessageId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
