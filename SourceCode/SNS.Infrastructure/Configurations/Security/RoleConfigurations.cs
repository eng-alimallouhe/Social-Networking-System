using SNS.Domain.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SNS.Infrastructure.Configurations.Security;

public class RoleConfigurations : 
    IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("nvarchar(50)");
    }
}