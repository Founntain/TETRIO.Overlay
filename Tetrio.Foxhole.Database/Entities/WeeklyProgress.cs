using System.ComponentModel.DataAnnotations.Schema;

namespace Tetrio.Foxhole.Database.Entities;

public class WeeklyProgress : BaseEntity
{
    public bool IsCompleted { get; set; } = false;

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    [ForeignKey("WeeklyChallengeId")]
    public Guid WeeklyChallengeId { get; set; }

    public virtual User User { get; set; }
    public virtual WeeklyChallenge WeeklyChallenge { get; set; }

    public virtual ISet<WeeklyConditionProgress> ConditionProgresses { get; set; } = new HashSet<WeeklyConditionProgress>();
}

public class WeeklyConditionProgress : BaseEntity
{
    public double CurrentProgress { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;

    [ForeignKey("WeeklyProgressId")]
    public Guid WeeklyProgressId { get; set; }

    [ForeignKey("WeeklyChallengeConditionId")]
    public Guid ConditionId { get; set; }

    public virtual WeeklyProgress WeeklyProgress { get; set; }
    public virtual WeeklyChallengeCondition WeeklyChallengeCondition { get; set; }
}