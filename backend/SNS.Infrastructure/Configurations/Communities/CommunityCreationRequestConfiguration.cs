using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Communities.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Infrastructure.Configurations.Communities;

public class CommunityCreationRequestConfiguration : IEntityTypeConfiguration<CommunityCreationRequest>
{
    public void Configure(EntityTypeBuilder<CommunityCreationRequest> builder)
    {
        builder.ToTable("CommunityCreationRequests");

        builder.HasKey(cc => cc.Id);

        builder.Property(cc => cc.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(cc => cc.Description)
            .HasMaxLength(500); 
        
        builder.Property(cc => cc.SubmitterId)
            .IsRequired();
        
        builder.Property(cc => cc.RequestedAt)
            .IsRequired();


        builder.HasOne<Profile>()
            .WithMany()
            .HasForeignKey(cc => cc.SubmitterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

