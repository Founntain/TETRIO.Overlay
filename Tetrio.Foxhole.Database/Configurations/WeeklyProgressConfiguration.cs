using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class WeeklyProgressConfiguration : BaseConfiguration<WeeklyProgress>
{
    public override void Configure(EntityTypeBuilder<WeeklyProgress> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new { x.UserId, x.WeeklyChallengeId }).IsUnique();

        builder.HasOne(x => x.User).WithMany(x => x.WeeklyProgressions).HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.WeeklyChallenge).WithMany(x => x.WeeklyProgressions).HasForeignKey(x => x.WeeklyChallengeId);
        builder.HasMany(x => x.ConditionProgresses).WithOne(x => x.WeeklyProgress).HasForeignKey(x => x.WeeklyProgressId).OnDelete(DeleteBehavior.Cascade);
    }
}