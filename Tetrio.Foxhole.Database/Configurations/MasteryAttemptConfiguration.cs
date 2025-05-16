using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class MasteryAttemptConfiguration : BaseConfiguration<MasteryAttempt>
{
    public override void Configure(EntityTypeBuilder<MasteryAttempt> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.User).WithMany(x => x.MasteryAttempts).HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.MasteryChallenge).WithMany(x => x.MasteryAttempts).HasForeignKey(x => x.MasteryChallengeId);
    }
}