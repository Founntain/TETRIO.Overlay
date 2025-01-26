using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Overlay.Database.Entities;

namespace Tetrio.Overlay.Database.Configurations;

public class UserConfiguration : BaseConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.SessionToken).IsUnique();
        builder.HasIndex(x => x.TetrioId).IsUnique();
        builder.HasIndex(x => x.DiscordId).IsUnique();
    }
}