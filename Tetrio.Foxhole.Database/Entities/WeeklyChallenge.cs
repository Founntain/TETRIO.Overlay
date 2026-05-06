using Tetrio.Foxhole.Base.Enums;

namespace Tetrio.Foxhole.Database.Entities;

public class WeeklyChallenge : BaseEntity
{
    public byte Week { get; set; }
    public DateOnly StartDate { get; set; }
    // public DateOnly EndDate { get; set; }
    public Mods Mods { get; set; }

    public virtual ISet<WeeklyChallengeCondition> Conditions { get; set; } = new HashSet<WeeklyChallengeCondition>();
    public virtual ISet<WeeklyProgress> WeeklyProgressions { get; set; } = new HashSet<WeeklyProgress>();
}