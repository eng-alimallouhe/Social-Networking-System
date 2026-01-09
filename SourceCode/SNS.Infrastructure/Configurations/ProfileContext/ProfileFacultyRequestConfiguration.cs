using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Education.Entities;

namespace SNS.Infrastructure.Configurations.ProfileContext
{
    public class ProfileFacultyRequestConfiguration : IEntityTypeConfiguration<ProfileFacultyRequest>
    {
        public void Configure(EntityTypeBuilder<ProfileFacultyRequest> builder)
        {
            builder.ToTable("ProfileFacultyRequests");

            builder.HasKey(pfr => pfr.Id);

            builder.HasIndex(pfr => pfr.JoinerId);
            builder.HasIndex(pfr => pfr.FacultyRequestId);

            builder.HasIndex(
                pfr => new 
                { 
                    pfr.JoinerId, 
                    pfr.FacultyRequestId
                })
                .IsUnique();

            builder.HasOne(pfr => pfr.Joiner)
                   .WithMany()
                   .HasForeignKey(pfr => pfr.JoinerId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pfr => pfr.FacultyRequest)
                   .WithMany(fr => fr.Requests)
                   .HasForeignKey(pfr => pfr.FacultyRequestId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}