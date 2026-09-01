using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Support.Entities;

namespace SNS.Infrastructure.Support.Configurations;

public class TicketMessageConfiguration : IEntityTypeConfiguration<TicketMessage>
{
    public void Configure(EntityTypeBuilder<TicketMessage> builder)
    {
        builder.ToTable("TicketMessages", "Support");

        builder.HasKey(m => m.Id);

        builder.HasIndex(m => m.TicketId);
        builder.HasIndex(m => m.SenderId);

        builder.Property(m => m.MessageBody)
               .IsRequired()
               .HasMaxLength(2000)
               .HasColumnType("nvarchar(2000)");

        builder.HasMany(m => m.Attachments)
               .WithOne()
               .HasForeignKey(a => a.TicketMessageId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
