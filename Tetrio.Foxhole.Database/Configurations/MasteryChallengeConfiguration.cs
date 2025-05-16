using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class MasteryChallengeConfiguration : BaseConfiguration<MasteryChallenge>
{
    public override void Configure(EntityTypeBuilder<MasteryChallenge> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.Date).IsUnique();

        builder.HasMany(x => x.Conditions).WithOne(x => x.MasteryChallenge).HasForeignKey(x => x.ChallengeId);
    }
}