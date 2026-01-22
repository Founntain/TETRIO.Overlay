using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class LeaderboardConfiguration : BaseConfiguration<Leaderboard>
{
    public override void Configure(EntityTypeBuilder<Leaderboard> builder)
    {
        base.Configure(builder);

        builder.HasMany(x => x.Entries).WithOne(x => x.Leaderboard);
    }
}