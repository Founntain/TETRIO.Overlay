using Tetrio.Overlay.Database.Enums;

namespace Tetrio.Overlay.Database.Entities;

public class ChallengeCondition : BaseEntity
{
    public Guid ChallengeId { get; set; }
    public ConditionType Type { get; set; }
    public int Value { get; set; }

    public virtual Challenge? Challenge { get; set; }

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
            default:
                return base.ToString();

        }
    }
}