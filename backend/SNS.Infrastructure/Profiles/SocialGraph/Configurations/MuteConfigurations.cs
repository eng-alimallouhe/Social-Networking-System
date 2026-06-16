using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.SocialGraph.Entities;

namespace SNS.Infrastructure.Profiles.SocialGraph.Configurations;

public class MuteConfigurations : IEntityTypeConfiguration<Mute>
{
    public void Configure(EntityTypeBuilder<Mute> builder)
    {
        builder.ToTable("Mutes", "Profiles");

        builder.HasKey(x => x.Id);

        builder.HasIndex(
            b => new
            {
                b.MuterId,
                b.MutedId
            }).IsUnique();

        builder.HasIndex(b => b.MutedId);


        builder.HasOne(x => x.Muted)
            .WithMany()
            .HasForeignKey(x => x.MutedId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Muter)
            .WithMany()
            .HasForeignKey(x => x.MuterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
