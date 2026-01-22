using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class LeaderboardEntryConfiguration : BaseConfiguration<LeaderboardEntry>
{
    public override void Configure(EntityTypeBuilder<LeaderboardEntry> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Leaderboard).WithMany(x => x.Entries);
        builder.HasOne(x => x.User).WithMany(x => x.LeaderboardEntries);
    }
}