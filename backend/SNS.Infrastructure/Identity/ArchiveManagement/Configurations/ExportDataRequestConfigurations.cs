using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Identity.ArchiveManagement.Configurations;

public class ExportDataRequestConfigurations : IEntityTypeConfiguration<ExportDataRequest>
{
    public void Configure(EntityTypeBuilder<ExportDataRequest> builder)
    {
        builder.ToTable("ExportDataRequest", "Identity");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(edr => edr.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
