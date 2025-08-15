namespace Tetrio.Foxhole.Database.Entities;

/// <summary>
/// Represents a user for Zenith Daily Challenge
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// The users TETR.IO unique ID
    /// </summary>
    public string TetrioId { get; set; }
    /// <summary>
    /// The users TETR.IO username
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The users current SessionToken
    /// </summary>
    public Guid? SessionToken { get; set; }

    /// <summary>
    /// The timestamp when the user request a submission of scores
    /// </summary>
    public DateTime? LastSubmission { get; set; }

    /// <summary>
    /// If the user is restricted? If so, they can't submit runs
    /// </summary>
    public bool IsRestricted { get; set; } = false;

    /// <summary>
    /// There current Tetra League Rank
    /// </summary>
    public string? TetrioRank { get; set; }

    /// <summary>
    /// The users ZDC Score
    /// </summary>
    public ulong Score { get; set; } = 0;

    /// <summary>
    /// The users ZDC Score before it got replaced by new scoring
    /// </summary>
    public ulong LegacyScore { get; set; } = 0;

    #region Discord Auth related Stuff

    /// <summary>
    /// The users discord ID linked to their TETR.IO account
    /// </summary>
    public string DiscordId { get; set; }

    /// <summary>
    /// The users discord AccessToken
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// The users discord RefreshToken
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// When the users discord tokens expire
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    #endregion

    public virtual ISet<Challenge> Challenges { get; set; } = new HashSet<Challenge>();
    public virtual ISet<ZenithSplit> Splits { get; set; } = new HashSet<ZenithSplit>();
    public virtual ISet<Run> Runs { get; set; } = new HashSet<Run>();
    public virtual ISet<CommunityContribution> CommunityContributions { get; set; } = new HashSet<CommunityContribution>();
    public virtual ISet<MasteryAttempt> MasteryAttempts { get; set; } = new HashSet<MasteryAttempt>();
}