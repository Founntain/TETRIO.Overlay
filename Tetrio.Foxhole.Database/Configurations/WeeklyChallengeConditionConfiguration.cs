using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class WeeklyChallengeConditionConfiguration : BaseConfiguration<WeeklyChallengeCondition>
{
    public override void Configure(EntityTypeBuilder<WeeklyChallengeCondition> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.WeeklyChallengeId);

        builder.HasOne(x => x.WeeklyChallenge).WithMany(x => x.Conditions).HasForeignKey(x => x.WeeklyChallengeId);
    }
}