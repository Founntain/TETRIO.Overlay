using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Enums;
using Tetrio.Foxhole.Network.Api.Tetrio;

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

    protected (double Alitude, double Vs, double Apm) CalculateNerfAdjustmentFactors(ConditionType type, string[] mods)
    {
        var scaling = 1d;


    }

    protected async Task<double> GetWorldRecordOfMod(Mods? mod = null)
    {
        var tetrioApi = new TetrioApi();

        var achievementId = mod switch
        {
            Mods.Expert => ModAchievements.Expert,
            Mods.NoHold => ModAchievements.NoHold,
            Mods.Messy => ModAchievements.Messy,
            Mods.Gravity => ModAchievements.Gravity,
            Mods.Volatile => ModAchievements.Volatile,
            Mods.DoubleHole => ModAchievements.DoubleHole,
            Mods.Invisible => ModAchievements.Invisible,
            Mods.AllSpin => ModAchievements.AllSpin,
            _ => ModAchievements.NoMod
        };

        var achievementData = await tetrioApi.GetAchievement(achievementId);

        return achievementData?.Leaderboard?.OrderByDescending(x => x.Value).FirstOrDefault()?.Value ?? -1;
    }
}