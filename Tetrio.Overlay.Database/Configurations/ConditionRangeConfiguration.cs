using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Overlay.Database.Entities;

namespace Tetrio.Overlay.Database.Configurations;

public class ConditionRangeConfiguration : BaseConfiguration<ConditionRange>
{
    public override void Configure(EntityTypeBuilder<ConditionRange> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new { x.ConditionType, x.Difficulty }).IsUnique();
    }
}