using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Foxhole.Database.Entities;

public abstract class ChallengeConditionBase : BaseEntity
{
    public Guid ChallengeId { get; set; }
    public ConditionType Type { get; set; }
    public double Value { get; set; }

    public override string ToString()
    {
        switch (Type)
        {
            case ConditionType.Height:
                return $"REACH {Value} M";
            case ConditionType.Spins:
                return $"DO {Value} SPINS";
            case ConditionType.AllClears:
                return $"DO {Value} ALL CLEARS";
            case ConditionType.KOs:
                return $"DO {Value} KO'S";
            case ConditionType.Quads:
                return $"DO {Value} QUADS";
            case ConditionType.Apm:
                return $"DO {Value} APM";
            case ConditionType.Pps:
                return $"DO {Value} PPS";
            case ConditionType.Vs:
                return $"DO {Value} VS";
            case ConditionType.Finesse:
                return $"DO {Value} % FINESSE";
            default:
                return base.ToString();

        }
    }
}

public class ChallengeCondition : ChallengeConditionBase
{
    public virtual Challenge? Challenge { get; set; }
}

public class MasteryChallengeCondition : ChallengeConditionBase
{
    public bool IsReverse { get; set; } = false;

    public virtual MasteryChallenge? MasteryChallenge { get; set; }
}