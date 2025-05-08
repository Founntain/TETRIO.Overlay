using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Foxhole.Database.Entities;

public class ConditionRange : BaseEntity
{
    public ConditionType ConditionType { get; set; }
    public Difficulty Difficulty { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
}