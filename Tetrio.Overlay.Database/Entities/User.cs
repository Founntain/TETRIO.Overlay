namespace Tetrio.Overlay.Database.Entities;

public class User : BaseEntity
{
    public string TetrioId { get; set; }
    public string Username { get; set; }

    public Guid SessionToken { get; set; }
    public DateTime? LastSubmission { get; set; }

    public bool IsRestricted { get; set; } = false;

    public string DiscordId { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }

    public virtual ISet<Challenge> Challenges { get; set; } = new HashSet<Challenge>();
    public virtual ISet<ZenithSplit> Splits { get; set; } = new HashSet<ZenithSplit>();
    public virtual ISet<Run> Runs { get; set; } = new HashSet<Run>();
    public virtual ISet<CommunityContribution> CommunityContributions { get; set; } = new HashSet<CommunityContribution>();
}