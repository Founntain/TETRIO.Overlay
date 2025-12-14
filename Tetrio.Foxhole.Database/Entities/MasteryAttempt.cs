namespace Tetrio.Foxhole.Database.Entities;

public class MasteryAttempt : BaseEntity
{
    public bool ExpertCompleted { get; set; } = false;
    public bool NoHoldCompleted { get; set; } = false;
    public bool MessyCompleted { get; set; } = false;
    public bool GravityCompleted { get; set; } = false;
    public bool VolatileCompleted { get; set; } = false;
    public bool DoubleHoleCompleted { get; set; } = false;
    public bool InvisibleCompleted { get; set; } = false;
    public bool AllSpinCompleted { get; set; } = false;

    public bool ExpertReversedCompleted { get; set; } = false;
    public bool NoHoldReversedCompleted { get; set; } = false;
    public bool MessyReversedCompleted { get; set; } = false;
    public bool GravityReversedCompleted { get; set; } = false;
    public bool VolatileReversedCompleted { get; set; } = false;
    public bool DoubleHoleReversedCompleted { get; set; } = false;
    public bool InvisibleReversedCompleted { get; set; } = false;
    public bool AllSpinReversedCompleted { get; set; } = false;

    public Guid UserId { get; set; }
    public Guid MasteryChallengeId { get; set; }

    public virtual User? User { get; set; }
    public virtual MasteryChallenge? MasteryChallenge { get; set; }
}