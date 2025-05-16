namespace Tetrio.Foxhole.Database.Entities;

public class MasteryChallenge : BaseEntity
{
    public DateOnly Date { get; set; }

    public virtual ISet<MasteryChallengeCondition> Conditions { get; set; } = new HashSet<MasteryChallengeCondition>();
    public virtual ISet<MasteryAttempt> MasteryAttempts { get; set; } = new HashSet<MasteryAttempt>();
}