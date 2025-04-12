using Tetrio.Overlay.Database.Enums;

namespace Tetrio.Overlay.Database.Entities;

public class CommunityChallenge : BaseEntity
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ConditionType ConditionType { get; set; }
    public double TargetValue { get; set; }
    public double Value { get; set; } = 0;
    public bool Finished { get; set; }

    public virtual ISet<CommunityContribution> Contributions { get; set; } = new HashSet<CommunityContribution>();
}