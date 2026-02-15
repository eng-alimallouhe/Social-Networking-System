using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Communities.Entities;

namespace SNS.Infrastructure.Configurations.Communities;

public class CommunityInvitationConfigurations :
    IEntityTypeConfiguration<CommunityInvitation>
{
    public void Configure(
        EntityTypeBuilder<CommunityInvitation> builder)
    {
        builder.ToTable("CommunityInvitations");

        builder.HasKey(ci => ci.Id);

        builder.HasIndex(ci => ci.CommunityId);
        builder.HasIndex(ci => ci.InviterId);
        builder.HasIndex(ci => ci.InviteeId)
            .IsUnique();

        // Optional: Unique constraint to prevent duplicate pending invitations?
        // Usually good to enforce uniqueness on pending ones, but let's keep standard index for now.


        builder.Property(ci => ci.Status).HasConversion<int>();

        builder.HasOne(ci => ci.Community)
               .WithMany() // Assuming Community doesn't track *invitations* explicitly in a collection (entity says 'Invitations' collection is missing in Community class snippet provided, or I missed it. If it exists, add logic)
               .HasForeignKey(ci => ci.CommunityId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.Inviter)
               .WithMany()
               .HasForeignKey(ci => ci.InviterId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ci => ci.Invitee)
               .WithMany()
               .HasForeignKey(ci => ci.InviteeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}