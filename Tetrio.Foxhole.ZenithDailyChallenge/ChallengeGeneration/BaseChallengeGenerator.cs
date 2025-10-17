using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Zenith.DailyChallenge.ChallengeGeneration;

public abstract class BaseChallengeGenerator
{
    protected Random _random;
    protected DateTime _day;

    public BaseChallengeGenerator(Random random, DateTime day)
    {
        _random = random;
        _day = day;
    }

    protected List<ConditionType> GetRandomConditions(string mods)
    {
        var allConditions = Enum.GetValues<ConditionType>().ToList();

        allConditions.RemoveAt(0);
        // Removed finesse for challenge generator because of popular request
        allConditions.Remove(ConditionType.Finesse);
        allConditions.Remove(ConditionType.TotalBonus); // Removed for now as balancing is not done yet

        // If no hold was selected as mod, remove all clears from the condition roll
        if(mods.Contains("nohold")) allConditions.Remove(ConditionType.AllClears);

        allConditions = allConditions.OrderBy(_ => _random.Next()).ToList();

        var selectedConditions = allConditions.Take(_random.Next(2, 4)).ToList();

        return selectedConditions;
    }

    protected List<ConditionType> GetRandomConditionsWithoutAllClears()
    {
        var allConditions = Enum.GetValues<ConditionType>().ToList();

        allConditions.RemoveAt(0);
        allConditions.Remove(ConditionType.Finesse);

        allConditions.Remove(ConditionType.AllClears);

        allConditions = allConditions.OrderBy(_ => _random.Next()).ToList();

        var selectedConditions = allConditions.Take(_random.Next(1, 3)).ToList();

        return selectedConditions;
    }

    protected async Task<(double min, double max)> GetRangeForConditionAndDifficulty(TetrioContext context, ConditionType condition, Difficulty difficulty)
    {
        var range = await context.ConditionRanges.FirstOrDefaultAsync(x => x.ConditionType == condition && x.Difficulty == difficulty);

        if(range == null)
            return (0, 0);

        return (range.Min, range.Max);
    }

    protected (double Alitude, double Vs, double Apm) CalculateNerfAdjustmentFactors(string[] mods)
    {
        var modCount = mods.Length;
        var altitudeAdjustment = 1d;
        var vsAdjustment = 1d;
        var apmAdjustment = 1d;

        if (mods.Contains("nohold"))
        {
            if (modCount == 1)
            {
                // 10% reduction
                altitudeAdjustment = 0.9d;
                vsAdjustment = 0.9d;
                apmAdjustment = 0.9d;
            }

            if (modCount == 2)
            {
                // Base Adjustment | 15% reduction
                altitudeAdjustment = 0.85d;
                vsAdjustment = 0.85d;
                apmAdjustment = 0.85d;

                if (mods.Contains("doublehole"))
                {
                    // 20% reduction for altitude and APM | 30% reduction for VS
                    if(altitudeAdjustment > 0.8d) altitudeAdjustment = 0.8d;
                    if(vsAdjustment > 0.70d) vsAdjustment = 0.70d;
                    if(apmAdjustment > 0.8d) apmAdjustment = 0.8d;
                }

                if (mods.Contains("gravity"))
                {
                    // 15% reduction for altitude | 20% reduction for APM & VS
                    if(altitudeAdjustment > 0.8d) altitudeAdjustment = 0.85d;
                    if(vsAdjustment > 0.8d) vsAdjustment = 0.8d;
                    if(apmAdjustment > 0.8d) apmAdjustment = 0.8d;
                }
            }

            if (modCount >= 3)
            {
                // 25% reduction
                if(altitudeAdjustment > 0.75d) altitudeAdjustment = 0.75d;
                if(vsAdjustment > 0.75d) vsAdjustment = 0.75d;
                if(apmAdjustment > 0.75d) apmAdjustment = 0.75d;
            }
        }

        return (altitudeAdjustment, vsAdjustment, apmAdjustment);
    }
}