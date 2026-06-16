using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Identity.Users.Configurations;
public class UserConfigurations : 
    IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "Identity");

        builder.HasKey(u => u.Id);

        // Indexes for frequent lookups
        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.Email);
        builder.HasIndex(u => u.RoleId);

        builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");



        builder.Property(u => u.Email)
                .HasMaxLength(20)
                .HasColumnType("varchar(20)");

        builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnType("nvarchar(512)");

        // Relationship: User -> Role
        builder.HasOne<Role>(u => u.Role)
                .WithMany() 
                .HasForeignKey(u => u.RoleId)
                .IsRequired();
    }
}
