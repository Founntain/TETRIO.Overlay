﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class CommunityContributionConfiguration : BaseConfiguration<CommunityContribution>
{
    public override void Configure(EntityTypeBuilder<CommunityContribution> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.User).WithMany(x => x.CommunityContributions).HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.CommunityChallenge).WithMany(x => x.Contributions).HasForeignKey(x => x.CommunityChallengeId);
    }
}