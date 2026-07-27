using System.ComponentModel.DataAnnotations.Schema;
using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Foxhole.Database.Entities;

public class Progression : BaseEntity
{
    public DateTime PlayedAt { get; set; }
    public ProgressionType Type { get; set; }
    public double Value { get; set; }
    public ZenithFloor Floor { get; set; }
    public bool IsPersonalBest { get; set; } = false;

    public string? Mods { get; set; }

    public string TetrioId { get; set; }

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public virtual User User { get; set; }
}