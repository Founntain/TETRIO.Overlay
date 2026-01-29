namespace Tetrio.Zenith.DailyChallenge.ChallengeGeneration;

[Flags]
public enum Mods : byte
{
    NoMod      = 0b00000000,
    Expert     = 0b00000001,
    NoHold     = 0b00000010,
    Messy      = 0b00000100,
    Gravity    = 0b00001000,
    Volatile   = 0b00010000,
    DoubleHole = 0b00100000,
    Invisible  = 0b01000000,
    AllSpin    = 0b10000000
}

public enum ReverseMods
{
    Expert     = 0b00000001,
    NoHold     = 0b00000010,
    Messy      = 0b00000100,
    Gravity    = 0b00001000,
    Volatile   = 0b00010000,
    DoubleHole = 0b00100000,
    Invisible  = 0b01000000,
    AllSpin    = 0b10000000
}

public static class ModSets
{
    public const Mods ModernClassic = Mods.NoHold | Mods.Gravity;
    public const Mods Deadlock = Mods.NoHold | Mods.Messy | Mods.DoubleHole;
    public const Mods EscapeArtist = Mods.Messy | Mods.DoubleHole | Mods.AllSpin;
    public const Mods StarvingArtist = Mods.NoHold | Mods.AllSpin;
    public const Mods ConArtist = Mods.Expert | Mods.Volatile | Mods.AllSpin;
    public const Mods TheGrandmaster = Mods.Gravity | Mods.Invisible;
    public const Mods EmperorsDecadence = Mods.Expert | Mods.NoHold | Mods.DoubleHole;
    public const Mods DivineMastery = Mods.Expert | Mods.Messy | Mods.Volatile | Mods.DoubleHole;
    public const Mods SwampWater = Mods.Expert | Mods.NoHold | Mods.Messy | Mods.Gravity | Mods.Volatile | Mods.DoubleHole | Mods.Invisible | Mods.AllSpin;
}

public static class ModUtils
{
    public static Mods ToMod(this string modString)
    {
        return modString switch
        {
            "expert" => Mods.Expert,
            "nohold" => Mods.NoHold,
            "messy" => Mods.Messy,
            "gravity" => Mods.Gravity,
            "volatile" => Mods.Volatile,
            "doublehole" => Mods.DoubleHole,
            "invisible" => Mods.Invisible,
            "allspin" => Mods.AllSpin,
            _ => Mods.NoMod,
        };
    }
}

/// <summary>
/// Contains the Achievement IDs from the game
/// </summary>
public static class ModAchievements
{
    public const string NoMod = "18";
    public const string Expert = "19";
    public const string NoHold = "23";
    public const string Messy = "24";
    public const string Gravity = "22";
    public const string Volatile = "21";
    public const string DoubleHole = "20";
    public const string Invisible = "25";
    public const string AllSpin = "26";

    // Modsets
    public const string ModernClassic = "28";
    public const string Deadlock = "29";
    public const string TheEscapeArtist = "33";
    public const string TheStarvingArtist = "44";
    public const string TheConArtist = "49";
    public const string TheGrandmaster = "30";
    public const string EmperorsDecadence = "31";
    public const string DivineMastery = "32";
    public const string SwampWater = "34";
}