namespace Tetrio.Foxhole.Database.Entities;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class CreationTimeEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}