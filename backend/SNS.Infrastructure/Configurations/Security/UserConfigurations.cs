using global::SNS.Domain.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SNS.Infrastructure.Configurations.Security;
public class UserConfigurations : 
    IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        // Indexes for frequent lookups
        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.PhoneNumber);
        builder.HasIndex(u => u.RoleId);

        builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

        builder.Property(u => u.Email)
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

        builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnType("varchar(20)");

        builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnType("nvarchar(512)");

        builder.Property(u => u.SecurityCode)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("varchar(10)");

        // Relationship: User -> Role
        builder.HasOne(u => u.Role)
                .WithMany() // Assuming Role doesn't have a Users collection based on your class
                .HasForeignKey(u => u.RoleId)
                .IsRequired();
    }
}