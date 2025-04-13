using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Overlay.Database.Entities;

namespace Tetrio.Overlay.Database.Configurations;

public class ChallengeConditionConfiguration : BaseConfiguration<ChallengeCondition>
{
    public override void Configure(EntityTypeBuilder<ChallengeCondition> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.ChallengeId);

        builder.HasOne(x => x.Challenge).WithMany(x => x.Conditions).HasForeignKey(x => x.ChallengeId);
    }
}