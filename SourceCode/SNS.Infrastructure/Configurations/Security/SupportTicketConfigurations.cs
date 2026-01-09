using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Security.Entities;

namespace SNS.Infrastructure.Configurations.Security;

public class SupportTicketConfigurations :
    IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("SupportTickets");

        builder.HasKey(st => st.Id);
        builder.HasIndex(st => st.ApplicantId);
        builder.HasIndex(st => st.ClosedBy);

        builder.Property(st => st.Description)
               .IsRequired()
               .HasMaxLength(2000)
               .HasColumnType("nvarchar(2000)");

        builder.Property(st => st.Status)
               .HasConversion<int>(); // Explicitly mapping enum to int

        builder.Property(st => st.Type)
               .HasConversion<int>();

        // JSON Conversion matching your SupportResponse style
        builder.Property(st => st.Attachments)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                   v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!)!)
               .HasColumnType("nvarchar(max)");

        builder.Property(st => st.Attachments).Metadata.SetValueComparer(
           new ValueComparer<List<string>>(
               (c1, c2) => c1!.SequenceEqual(c2!),
               c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
               c => c.ToList()));

        // Relationships
        builder.HasOne<User>()
               .WithMany(u => u.SupportTickets)
               .HasForeignKey(st => st.ApplicantId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}