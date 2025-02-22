using Microsoft.EntityFrameworkCore;
using Tetrio.Overlay.Database.Configurations;
using Tetrio.Overlay.Database.Entities;

namespace Tetrio.Overlay.Database;

public class TetrioContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Challenge> Challenges { get; set; }
    public DbSet<ZenithSplit> ZenithSplits { get; set; }
    public DbSet<ConditionRange> ConditionRanges { get; set; }
    public DbSet<ChallengeCondition> ChallengeConditions { get; set; }
    public DbSet<Mod> Mods { get; set; }

    public TetrioContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
        optionsBuilder.UseLazyLoadingProxies();

        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Seed();

        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new ZenithSplitConfiguration());
        builder.ApplyConfiguration(new ChallengeConfiguration());
        builder.ApplyConfiguration(new ChallengeConditionConfiguration());
        builder.ApplyConfiguration(new ConditionRangeConfiguration());
        builder.ApplyConfiguration(new ModConfiguration());
    }
}