using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class WeeklyChallengeConfiguration : BaseConfiguration<WeeklyChallenge>
{
    public override void Configure(EntityTypeBuilder<WeeklyChallenge> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.StartDate).IsUnique();

        builder.HasMany(x => x.Conditions).WithOne(x => x.WeeklyChallenge).HasForeignKey(x => x.WeeklyChallengeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.WeeklyProgressions).WithOne(x => x.WeeklyChallenge).HasForeignKey(x => x.WeeklyChallengeId).OnDelete(DeleteBehavior.Cascade);
    }
}