using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Identity.ArchiveManagement.Configurations
{
    public class UserArchiveConfigurations : IEntityTypeConfiguration<UserArchive>
    {
        public void Configure(EntityTypeBuilder<UserArchive> builder)
        {
            builder.ToTable("UserArchives", "Identity");

            builder.HasKey(ua => ua.Id);
            builder.HasIndex(ua => ua.TargetId);
            builder.HasIndex(ua => ua.PerformedById);

            builder.Property(ua => ua.Reason)
                   .HasMaxLength(500)
                   .HasColumnType("nvarchar(500)");

            
            builder.HasOne<User>()
                   .WithMany(u => u.Archives)
                   .HasForeignKey(ua => ua.TargetId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                   .WithMany(u => u.ActionPerformed)
                   .HasForeignKey(ua => ua.PerformedById)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
