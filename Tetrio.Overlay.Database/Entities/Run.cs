namespace Tetrio.Overlay.Database.Entities;

public class Run : BaseEntity
{
    public string TetrioId { get; set; }

    // Run Data
    public double Altitude { get; set; } = 0;
    public byte KOs { get; set; } = 0;
    public ushort AllClears { get; set; } = 0;
    public ushort Quads { get; set; } = 0;
    public ushort Spins { get; set; } = 0;
    public string Mods { get; set; }

    public virtual ISet<Challenge>? Challenges { get; set; } = new HashSet<Challenge>();
    public virtual User User { get; set; }
}