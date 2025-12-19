using System.ComponentModel.DataAnnotations;
using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Foxhole.Database.Entities;

public class CommunityChallenge : BaseEntity
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ConditionType ConditionType { get; set; }
    public double TargetValue { get; set; }
    public double Value { get; set; } = 0;
    public bool Finished { get; set; }

    public string? Mods { get; set; } = null;
    public bool RequireAllMods { get; set; } = true;
    public bool ShowMods { get; set; } = true;

    [MaxLength(256)]
    public string? Name { get; set; }
    [MaxLength(4096)]
    public string? Description { get; set; }

    public virtual ISet<CommunityContribution> Contributions { get; set; } = new HashSet<CommunityContribution>();
}