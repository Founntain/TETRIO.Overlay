namespace Tetrio.Foxhole.Database.Entities;

public class Leaderboard : CreationTimeEntity
{
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }

    public virtual ISet<LeaderboardEntry> Entries { get; set; } = new HashSet<LeaderboardEntry>();
}

public class LeaderboardEntry : CreationTimeEntity
{
    public long Score { get; set; } = 0;

    public virtual Leaderboard Leaderboard { get; set; }
    public virtual User User { get; set; }
}