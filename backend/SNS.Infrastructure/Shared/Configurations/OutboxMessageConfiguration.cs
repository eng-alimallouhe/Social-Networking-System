using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Shared.Entities;

namespace SNS.Infrastructure.Shared.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages", "EventsHolder");

        builder.HasKey(om => om.Id);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.OccurredOnUtc)
            .IsRequired();

        builder.Property(x => x.ProcessedOnUtc)
            .IsRequired(false);

        builder.Property(x => x.Error)
            .IsRequired(false);

        builder.HasIndex(
            x => new 
            { 
                x.ProcessedOnUtc, 
                x.OccurredOnUtc 
            })
            .HasDatabaseName("IX_OutboxMessages_Unprocessed");
    }
}
