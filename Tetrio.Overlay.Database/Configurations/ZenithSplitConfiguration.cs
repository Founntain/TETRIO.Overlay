using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Overlay.Database.Entities;

namespace Tetrio.Overlay.Database.Configurations;

public class ZenithSplitConfiguration : BaseConfiguration<ZenithSplit>
{
    public override void Configure(EntityTypeBuilder<ZenithSplit> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.TetrioId).IsUnique();

        builder.HasOne(x => x.User).WithMany(x => x.Splits);
    }
}