using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class WeeklyConditionProgressConfiguration : BaseConfiguration<WeeklyConditionProgress>
{
    public override void Configure(EntityTypeBuilder<WeeklyConditionProgress> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new {x.WeeklyProgressId, WeeklyChallengeConditionId = x.ConditionId}).IsUnique();

        builder.HasOne(x => x.WeeklyProgress).WithMany(x => x.ConditionProgresses);
        builder.HasOne(x => x.WeeklyChallengeCondition).WithMany(x => x.ConditionProgress);
    }
}