using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Communities.Entities;

namespace SNS.Infrastructure.Configurations.Communities;

public class CommunityMembershipConfigurations :
    IEntityTypeConfiguration<CommunityMembership>
{
    public void Configure(EntityTypeBuilder<CommunityMembership> builder)
    {
        builder.ToTable("CommunityMemberships");

        builder.HasKey(cm => cm.Id);

        builder.HasIndex(cm => cm.CommunityId);
        
        builder.HasIndex(cm => cm.MemberId);

        builder.HasIndex(
            cm => new 
            { 
                cm.CommunityId, 
                cm.MemberId 
            })
            .IsUnique();

        builder.Property(cm => cm.Status)
            .HasConversion<int>();
        
        builder.Property(cm => cm.Role)
            .HasConversion<int>();

        builder.HasOne(cm => cm.Community)
               .WithMany(c => c.Memberships)
               .HasForeignKey(cm => cm.CommunityId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cm => cm.Member)
               .WithMany(p => p.CommunityMemberships) 
               .HasForeignKey(cm => cm.MemberId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}