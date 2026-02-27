using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Tetrio.Foxhole.Network.Api.Tetrio.Models;

namespace Tetrio.Foxhole.Database.Entities;

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
    public double Apm { get; set; } = 0;
    public double Pps { get; set; } = 0;
    public double Vs { get; set; } = 0;
    public double Finesse { get; set; } = 0;
    public ushort Back2Back { get; set; } = 0;
    public double TotalBonus { get; set; } = 0;

    // Meta data
    public uint LinesCleared { get; set; } = 0;
    public uint Inputs { get; set; } = 0;
    public uint Holds { get; set; } = 0;
    public uint Score { get; set; } = 0;
    public byte TopCombo { get; set; } = 0;
    public uint PiecesPlaced { get; set; } = 0;

    // Zenith Meta Data
    public double Rank { get; set; } = 0;
    public double PeakRank { get; set; } = 0;
    public double AverageRankPoints { get; set; } = 0;
    public byte Floor { get; set; } = 0;
    public double TargetingFactor { get; set; } = 0;
    public double TargetingGrace { get; set; } = 0;
    public string? GameOverReason { get; set; } = null;
    public ushort PeakPosition { get; set; } = 0;
    public ushort PeakPlayerCount{ get; set; } = 0;
    public ushort FinalPosition{ get; set; } = 0;
    public ushort FinalPlayerCount{ get; set; } = 0;

    // Garbage
    public uint GarbageSent { get; set; } = 0;
    public uint GarbageSendNoMult { get; set; } = 0;
    public uint GarbageMaxSpike { get; set; } = 0;
    public uint GarbageMaxSpikeNoMult { get; set; } = 0;
    public uint GarbageReceived { get; set; } = 0;
    public uint GarbageAttack { get; set; } = 0;
    public uint GarbageCleared { get; set; } = 0;

    public int TotalTime { get; set; } = 0;

    // Calculated Stats
    public double App { get; set; } = 0;

    public virtual ISet<Challenge>? Challenges { get; set; } = new HashSet<Challenge>();

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public virtual User User { get; set; }

    public static Run Create(User user, Record record, Stats stats, Clears clears, double finesse, double? totalSpins, string[] mods)
    {
        var app = 0d;

        if (stats?.Piecesplaced > 0 && stats?.Garbage?.Attack > 0)
            app = (stats.Piecesplaced / stats.Garbage.Attack).Value;

        return new Run
        {
            User = user,
            TetrioId = record.Id,
            PlayedAt = record.Ts,

            Altitude = stats.Zenith.Altitude ?? 0,
            KOs = (byte?)stats.Kills ?? 0,
            AllClears = (ushort?)clears.AllClear ?? 0,
            Quads = (ushort?)clears.Quads ?? 0,
            Spins = (ushort?)totalSpins ?? 0,
            Mods = string.Join(" ", mods),

            SpeedrunSeen = stats.Zenith.SpeedrunSeen ?? false,
            SpeedrunCompleted = stats.Zenith.Speedrun ?? false,

            Apm = record.Results.Aggregatestats.Apm ?? 0,
            Pps = record.Results.Aggregatestats.Pps ?? 0,
            Vs = record.Results.Aggregatestats.Vsscore ?? 0,
            Finesse = finesse,
            Back2Back = (ushort?)stats.Topbtb ?? 0,
            TotalBonus = stats.Zenith.Totalbonus ?? 0,

            LinesCleared = stats.Lines ?? 0,
            Inputs = stats.Inputs ?? 0,
            Holds = stats.Holds ?? 0,
            Score = (uint?) stats.Score ?? 0,
            TopCombo = (byte)(stats.Topcombo ?? 0),
            PiecesPlaced = (uint?) stats.Piecesplaced ?? 0,

            Rank = stats.Zenith.Rank ?? 0,
            PeakRank = stats.Zenith.Peakrank ?? 0,
            AverageRankPoints = stats.Zenith.Avgrankpts ?? 0,
            Floor = (byte?) stats.Zenith.Floor ?? 0,
            TargetingFactor = stats.Zenith.Targetingfactor ?? 0,
            TargetingGrace = stats.Zenith.Targetinggrace ?? 0,
            GameOverReason = record.Results.GameOverReason,

            GarbageSent = stats.Garbage.Sent ?? 0,
            GarbageSendNoMult = stats.Garbage.SentNomult ?? 0,
            GarbageMaxSpike = stats.Garbage.Maxspike ?? 0,
            GarbageMaxSpikeNoMult = stats.Garbage.MaxspikeNomult ?? 0,
            GarbageReceived = stats.Garbage.Received ?? 0,
            GarbageAttack = stats.Garbage.Attack ?? 0,
            GarbageCleared = stats.Garbage.Cleared ?? 0,

            TotalTime = (int)Math.Round(stats.Finaltime ?? 0, 0),

            PeakPosition = record.Extras.Zenith.PeakPos,
            PeakPlayerCount = record.Extras.Zenith.PeakCount,
            FinalPosition = record.Extras.Zenith.FinalPos,
            FinalPlayerCount = record.Extras.Zenith.FinalCount,

            App = app
        };
    }
}