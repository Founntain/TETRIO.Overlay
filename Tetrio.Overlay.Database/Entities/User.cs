namespace Tetrio.Overlay.Database.Entities;

public class User : BaseEntity
{
    public string TetrioId { get; set; }
    public string Username { get; set; }

    public Guid SessionToken { get; set; }

    public string DiscordId { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }

    public virtual ISet<Challenge>? Challenges { get; set; } = new HashSet<Challenge>();
    public virtual ISet<ZenithSplit>? ZenithSplits { get; set; } = new HashSet<ZenithSplit>();
}