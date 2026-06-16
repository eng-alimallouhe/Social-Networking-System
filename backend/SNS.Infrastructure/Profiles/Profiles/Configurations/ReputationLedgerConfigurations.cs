using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.Profiles.Profiles.Configurations;


public class ReputationLedgerConfigurations : IEntityTypeConfiguration<ReputationLedger>
{
    public void Configure(EntityTypeBuilder<ReputationLedger> builder)
    {
        builder.ToTable("ReputationLedgers", "Profiles");

        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.ProfileId);


        builder.Property(r => r.ActionType)
            .HasColumnType("int")
            .IsRequired();

        builder.HasOne<Profile>()
            .WithMany(profile => profile.ReputationHistory)
            .HasForeignKey(builder => builder.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
