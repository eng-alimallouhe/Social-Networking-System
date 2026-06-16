using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Identity.Users.Configurations;

public class RoleConfigurations : 
    IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "Identity");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Type)
                .IsRequired()
                .HasColumnType("int");
    }
}
