using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Security.Entities;

namespace SNS.Infrastructure.Configurations.Security
{
    public class PendingUpdateConfigurations :
        IEntityTypeConfiguration<PendingUpdate>
    {
        public void Configure(EntityTypeBuilder<PendingUpdate> builder)
        {
            builder.ToTable("PendingUpdates");

            builder.HasKey(pu => pu.Id);
            builder.HasIndex(pu => pu.UserId);

            // Search Index requested
            builder.HasIndex(pu => pu.UpdatedInfo);

            builder.Property(pu => pu.UpdatedInfo)
                   .IsRequired()
                   .HasMaxLength(1000)
                   .HasColumnType("nvarchar(1000)");

            builder.HasOne(pu => pu.User)
                   .WithMany(u => u.PendingUpdates)
                   .HasForeignKey(pu => pu.UserId)
                   .IsRequired();
        }
    }
}