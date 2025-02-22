using Tetrio.Overlay.Database.Enums;

namespace Tetrio.Overlay.Database.Entities;

public class ConditionRange : BaseEntity
{
    public ConditionType ConditionType { get; set; }
    public Difficulty Difficulty { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
}