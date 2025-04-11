namespace Tetrio.Overlay.Database.Entities;

public class CommunityContribution : BaseEntity
{
    public double Amount { get; set; }

    public Guid UserId { get; set; }
    public Guid CommunityChallengeId { get; set; }

    public virtual User User { get; set; }
    public virtual CommunityChallenge CommunityChallenge { get; set; }
}