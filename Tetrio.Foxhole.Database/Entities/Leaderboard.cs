using System.ComponentModel.DataAnnotations.Schema;

namespace Tetrio.Foxhole.Database.Entities;

public class Leaderboard : CreationTimeEntity
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }

    public virtual ISet<LeaderboardEntry> Entries { get; set; } = new HashSet<LeaderboardEntry>();
}

public class LeaderboardEntry : CreationTimeEntity
{
    public long Score { get; set; } = 0;

    [ForeignKey("LeaderboardId")]
    public Guid LeaderboardId { get; set; }
    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public virtual Leaderboard Leaderboard { get; set; }
    public virtual User User { get; set; }
}