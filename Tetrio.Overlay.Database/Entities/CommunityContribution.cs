namespace Tetrio.Overlay.Database.Entities;

public class CommunityContribution : BaseEntity
{
    public double Amount { get; set; }
    public bool IsLate { get; set; } = false;

    public Guid UserId { get; set; }
    public Guid CommunityChallengeId { get; set; }

    public virtual User User { get; set; }
    public virtual CommunityChallenge CommunityChallenge { get; set; }
}