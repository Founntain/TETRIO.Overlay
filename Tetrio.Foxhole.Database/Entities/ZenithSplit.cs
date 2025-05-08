namespace Tetrio.Foxhole.Database.Entities;

public class ZenithSplit : BaseEntity
{
    public string TetrioId { get; set; }

    public uint HotelReachedAt { get; set; }
    public uint CasinoReachedAt { get; set; }
    public uint ArenaReachedAt { get; set; }
    public uint MuseumReachedAt { get; set; }
    public uint OfficesReachedAt { get; set; }
    public uint LaboratoryReachedAt { get; set; }
    public uint CoreReachedAt { get; set; }
    public uint CorruptionReachedAt { get; set; }
    public uint PlatformOfTheGodsReachedAt { get; set; }

    public virtual User User { get; set; }
}