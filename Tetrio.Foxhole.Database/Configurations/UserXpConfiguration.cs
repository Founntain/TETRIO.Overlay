using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class UserXpConfiguration : BaseConfiguration<UserXp>
{
    public override void Configure(EntityTypeBuilder<UserXp> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.User).WithMany(x => x.Xp);
    }
}