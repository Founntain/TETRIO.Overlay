using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Foxhole.Database.Entities;

public class WeeklyChallengeCondition : BaseEntity
{
    public Guid WeeklyChallengeId { get; set; }
    public WeeklyConditionType Type { get; set; }
    public double Value { get; set; }

    public virtual WeeklyChallenge WeeklyChallenge { get; set; }
    public virtual ISet<WeeklyConditionProgress> ConditionProgress { get; set; } = new HashSet<WeeklyConditionProgress>();
}