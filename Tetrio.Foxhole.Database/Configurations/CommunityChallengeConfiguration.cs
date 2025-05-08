using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class CommunityChallengeConfiguration : BaseConfiguration<CommunityChallenge>
{
    public override void Configure(EntityTypeBuilder<CommunityChallenge> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.StartDate).IsUnique();
        builder.HasMany(x => x.Contributions).WithOne(x => x.CommunityChallenge).HasForeignKey(x => x.CommunityChallengeId);
    }
}