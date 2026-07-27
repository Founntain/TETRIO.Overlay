using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetrio.Foxhole.Database.Entities;

namespace Tetrio.Foxhole.Database.Configurations;

public class ProgressionConfiguration : BaseConfiguration<Progression>
{
    public override void Configure(EntityTypeBuilder<Progression> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.User).WithMany(x => x.Progressions).HasForeignKey(x => x.UserId);
    }
}