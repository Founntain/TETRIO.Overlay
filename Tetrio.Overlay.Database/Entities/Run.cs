namespace Tetrio.Overlay.Database.Entities;

public class Run : BaseEntity
{
    public string TetrioId { get; set; }
    public DateTime? PlayedAt { get; set; }

    // Run Data
    public double Altitude { get; set; } = 0;
    public byte KOs { get; set; } = 0;
    public ushort AllClears { get; set; } = 0;
    public ushort Quads { get; set; } = 0;
    public ushort Spins { get; set; } = 0;
    public string Mods { get; set; } = string.Empty;

    // Speedrun
    public bool SpeedrunSeen { get; set; } = false;
    public bool SpeedrunCompleted { get; set; } = false;

    //Other Stats
    // TODO: Condition Ideas
    // APM, PPS, Finesse, maybe Time?
    public double Apm { get; set; } = 0;
    public double Pps { get; set; } = 0;
    public double Vs { get; set; } = 0;
    public double Finesse { get; set; } = 0;


    public virtual ISet<Challenge>? Challenges { get; set; } = new HashSet<Challenge>();
    public virtual User User { get; set; }
}