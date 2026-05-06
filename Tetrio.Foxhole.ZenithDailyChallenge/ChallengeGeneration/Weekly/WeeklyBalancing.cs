using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Zenith.DailyChallenge.Controllers;

public class WeeklyBalancing
{
    public static Dictionary<WeeklyConditionType, (double Min, double Max)> Balancing = new()
    {
        // (min, max)
        { WeeklyConditionType.Height,         (15000, 30000) },
        { WeeklyConditionType.KOs,            (50, 100) },
        { WeeklyConditionType.Quads,          (150, 500) },
        { WeeklyConditionType.Spins,          (250, 1000) },
        { WeeklyConditionType.AllClears,      (50, 100) },
        { WeeklyConditionType.BackToBack,     (200, 800) },
        { WeeklyConditionType.TotalBonus,     (10000, 17500) },
        { WeeklyConditionType.LinesCleared,   (12000, 25000) },
        { WeeklyConditionType.GarbageSent,    (1000, 7500) },
        { WeeklyConditionType.GarbageCleared, (1000, 5000) },
    };
}