using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Communities.Entities;

namespace SNS.Infrastructure.Configurations.Communities;

public class CommunityRuleConfigurations :
    IEntityTypeConfiguration<CommunityRule>
{
    public void Configure(EntityTypeBuilder<CommunityRule> builder)
    {
        builder.ToTable("CommunityRules");

        builder.HasKey(cr => cr.Id);
        builder.HasIndex(cr => cr.CommunityId);

        builder.Property(cr => cr.Title)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(cr => cr.Description)
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");

        builder.HasOne(cr => cr.Community)
               .WithMany(c => c.Rules)
               .HasForeignKey(cr => cr.CommunityId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}