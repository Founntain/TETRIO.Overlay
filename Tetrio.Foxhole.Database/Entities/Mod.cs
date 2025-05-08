using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Foxhole.Database.Entities;

public class Mod : BaseEntity
{
    public string Name { get; set; }
    public Difficulty MinDifficulty { get; set; }
    public byte Weight { get; set; }
    public double Scaling { get; set; }
}