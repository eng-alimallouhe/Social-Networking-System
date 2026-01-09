using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Security.Entities;

namespace SNS.Infrastructure.Configurations.Security
{
    public class UserArchiveConfigurations : IEntityTypeConfiguration<UserArchive>
    {
        public void Configure(EntityTypeBuilder<UserArchive> builder)
        {
            builder.ToTable("UserArchives");

            builder.HasKey(ua => ua.Id);
            builder.HasIndex(ua => ua.UserId);
            builder.HasIndex(ua => ua.PerformedBy);

            builder.Property(ua => ua.Reason)
                   .HasMaxLength(500)
                   .HasColumnType("nvarchar(500)");

            
            builder.HasOne<User>()
                   .WithMany(u => u.Archives)
                   .HasForeignKey(ua => ua.UserId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                   .WithMany(u => u.ActionPerformed)
                   .HasForeignKey(ua => ua.PerformedBy)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}