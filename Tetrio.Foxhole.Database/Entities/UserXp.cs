using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Foxhole.Database.Entities;

public class UserXp : BaseEntity
{
    public long TotalXp { get; set; }
    public long Level { get; set; }
    public XpType Type { get; set; }

    public virtual User User { get; set; }

    public int CalculateLevel()
    {
        // Totally not the XP formula from TETR.IO

        var xp = Math.Max(0, TotalXp);

        var term1 = Math.Pow(xp / 500, 0.6);

        var extra = Math.Max(0, xp - 4_000_000) / 5000;
        var term2 = xp / (5000 + extra);

        return (int)(term1 + term2 + 1);
    }

    public static int CalculateLevelFromTotalXp(long totalXp)
    {
        // Totally not the XP formula from TETR.IO

        var xp = Math.Max(0, totalXp);

        var term1 = Math.Pow(xp / 500, 0.6);

        var extra = Math.Max(0, xp - 4_000_000) / 5000;
        var term2 = xp / (5000 + extra);

        return (int)(term1 + term2 + 1);
    }
}