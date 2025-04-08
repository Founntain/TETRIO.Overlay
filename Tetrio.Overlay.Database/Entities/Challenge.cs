namespace Tetrio.Overlay.Database.Entities;

public class Challenge : BaseEntity
{
    public DateOnly Date { get; set; }
    public string Mods { get; set; }
    public byte Points { get; set; }

    public virtual ISet<User> Users { get; set; } = new HashSet<User>();
    public virtual ISet<Run> Runs { get; set; } = new HashSet<Run>();
    public virtual ISet<ChallengeCondition> Conditions { get; set; } = new HashSet<ChallengeCondition>();
}