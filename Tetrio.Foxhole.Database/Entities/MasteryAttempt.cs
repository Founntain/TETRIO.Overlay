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

    public Guid UserId { get; set; }
    public Guid MasteryChallengeId { get; set; }

    public virtual User? User { get; set; }
    public virtual MasteryChallenge? MasteryChallenge { get; set; }
}