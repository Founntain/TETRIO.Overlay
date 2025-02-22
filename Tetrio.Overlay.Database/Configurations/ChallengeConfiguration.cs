using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Overlay.Database.Entities;

namespace Tetrio.Overlay.Database.Configurations;

public class ChallengeConfiguration : BaseConfiguration<Challenge>
{
    public override void Configure(EntityTypeBuilder<Challenge> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new {x.Date, x.Points}).IsUnique();
    }
}