using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.ContentManagement.Communities.Configurations;

public class CommunityInvitationConfigurations :
    IEntityTypeConfiguration<CommunityInvitation>
{
    public void Configure(
        EntityTypeBuilder<CommunityInvitation> builder)
    {
        builder.ToTable("CommunityInvitations", "Communities");

        builder.HasKey(ci => ci.Id);

        builder.HasIndex(ci => ci.CommunityId);
        builder.HasIndex(ci => ci.InviterId);
        builder.HasIndex(ci => ci.InviteeId);


        builder.HasIndex(ci => new
        {
            ci.CommunityId,
            ci.InviteeId
        }).IsUnique();

        builder.Property(ci => ci.Status).HasConversion<int>();

        builder.HasOne<Community>()
               .WithMany()
               .HasForeignKey(ci => ci.CommunityId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(ci => ci.InviterId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(ci => ci.InviteeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
