using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class RunConfiguration : BaseConfiguration<Run>
{
    public override void Configure(EntityTypeBuilder<Run> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.TetrioId).IsUnique();

        builder.HasOne(x => x.User).WithMany(x => x.Runs);
        builder.HasMany(x => x.Challenges).WithMany(x => x.Runs);
    }
}