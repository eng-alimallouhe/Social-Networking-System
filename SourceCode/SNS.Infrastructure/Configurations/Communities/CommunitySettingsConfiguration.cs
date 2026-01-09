using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Communities.Entities;

namespace SNS.Infrastructure.Configurations.Communities
{
    public class CommunitySettingsConfigurations : IEntityTypeConfiguration<CommunitySettings>
    {
        public void Configure(EntityTypeBuilder<CommunitySettings> builder)
        {
            builder.ToTable("CommunitySettings");

            builder.HasKey(cs => cs.Id);

            builder.HasIndex(cs => cs.CommunityId).IsUnique();
        }
    }
}