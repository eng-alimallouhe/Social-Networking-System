using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.QA.Entities;

namespace SNS.Infrastructure.Configurations.QA;

public class ProblemContentBlockConfigurations :
    IEntityTypeConfiguration<ProblemContentBlock>
{
    public void Configure(EntityTypeBuilder<ProblemContentBlock> builder)
    {
        builder.ToTable("ProblemContentBlocks");

        builder.HasKey(pcb => pcb.Id);
        builder.HasIndex(pcb => pcb.ProblemId);

        builder.Property(pcb => pcb.Type)
               .HasConversion<int>();

        builder.Property(pcb => pcb.Content)
               .IsRequired()
               .HasColumnType("nvarchar(max)");

        builder.Property(pcb => pcb.ExtraInfo)
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.HasOne(pcb => pcb.Problem)
               .WithMany(p => p.ContentBlocks)
               .HasForeignKey(pcb => pcb.ProblemId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}