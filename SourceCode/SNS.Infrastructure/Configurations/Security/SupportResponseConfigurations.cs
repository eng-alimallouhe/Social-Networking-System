using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Security.Entities;

namespace SNS.Infrastructure.Configurations.Security
{
    public class SupportResponseConfigurations :
        IEntityTypeConfiguration<SupportResponse>
    {
        public void Configure(EntityTypeBuilder<SupportResponse> builder)
        {
            builder.ToTable("SupportResponses");

            builder.HasKey(sr => sr.Id);
            builder.HasIndex(sr => sr.SenderId);
            builder.HasIndex(sr => sr.TicketId);
            builder.HasIndex(sr => sr.ParentResponseId);

            builder.Property(sr => sr.Message)
                   .IsRequired()
                   .HasMaxLength(2000)
                   .HasColumnType("nvarchar(2000)");

            // Converting List<string> to JSON for storage
            builder.Property(sr => sr.Attachments)
                   .HasConversion(
                       v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                       v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!)!)
                   .HasColumnType("nvarchar(max)");

            builder.Property(sr => sr.Attachments).Metadata.SetValueComparer(
               new ValueComparer<List<string>>(
                   (c1, c2) => c1!.SequenceEqual(c2!),
                   c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                   c => c.ToList()));

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(sr => sr.SenderId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<SupportTicket>()
                   .WithMany(t => t.Responses)
                   .HasForeignKey(sr => sr.TicketId)
                   .IsRequired();

            // Self-referencing relationship
            builder.HasOne<SupportResponse>()
                   .WithMany(sr => sr.Replies)
                   .HasForeignKey(sr => sr.ParentResponseId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}