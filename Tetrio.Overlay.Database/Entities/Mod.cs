using Tetrio.Overlay.Database.Enums;

namespace Tetrio.Overlay.Database.Entities;

public class Mod : BaseEntity
{
    public string Name { get; set; }
    public Difficulty MinDifficulty { get; set; }
    public byte Weight { get; set; }
}