using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ContentManagement.Communities.Entities;

namespace SNS.Infrastructure.ContentManagement.Communities.Configurations
{
    public class CommunitySettingsConfigurations : IEntityTypeConfiguration<CommunitySettings>
    {
        public void Configure(EntityTypeBuilder<CommunitySettings> builder)
        {
            builder.ToTable("CommunitySettings", "Communities");

            builder.HasKey(cs => cs.Id);

            builder.HasIndex(cs => cs.CommunityId).IsUnique();
        }
    }
}
