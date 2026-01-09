using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.QA.Entities;

namespace SNS.Infrastructure.Configurations.QA;

public class SolutionContentBlockConfigurations :
    IEntityTypeConfiguration<SolutionContentBlock>
{
    public void Configure(EntityTypeBuilder<SolutionContentBlock> builder)
    {
        builder.ToTable("SolutionContentBlocks");

        builder.HasKey(scb => scb.Id);

        builder.HasIndex(scb => scb.SolutionId);

        builder.Property(scb => scb.Type)
               .HasConversion<int>();

        builder.Property(scb => scb.Content)
               .IsRequired()
               .HasColumnType("nvarchar(max)");

        builder.Property(scb => scb.ExtraInfo)
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.HasOne(scb => scb.Solution)
               .WithMany(s => s.ContentBlocks)
               .HasForeignKey(scb => scb.SolutionId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}