using Microsoft.EntityFrameworkCore;
using Tetrio.Overlay.Database.Entities;
using Tetrio.Overlay.Database.Enums;

namespace Tetrio.Overlay.Database;

public class ChallengeGenerator
{
    private readonly Random _random;
    private readonly DateTimeOffset _day;

    public ChallengeGenerator()
    {
        _day = DateTimeOffset.UtcNow;

        var seed = int.Parse(_day.ToString("yyyyMMdd"));

        _random = new(seed);
    }

    public async Task<List<Challenge>> GenerateChallengesForDay(TetrioContext context)
    {
        var challenges = new List<Challenge>();

        challenges.Add(await GenerateChallenge(Difficulty.Easy, context));
        challenges.Add(await GenerateChallenge(Difficulty.Normal, context));
        challenges.Add(await GenerateChallenge(Difficulty.Hard, context));

        return challenges;
    }

    private async Task<Challenge> GenerateChallenge(Difficulty difficulty, TetrioContext context)
    {
        var challengeConditions = new List<ChallengeCondition>();

        // Always add Height as a base condition
        var heightRange = GetRangeForConditionAndDifficulty(context, ConditionType.Height, difficulty);
        var height = _random.Next(heightRange.min, heightRange.max + 1);

        challengeConditions.Add(new () { Type = ConditionType.Height, Value = height});

        var tries = 0;

        while (challengeConditions.Count == 1)
        {
            var conditions = GetRandomConditions();

            foreach (var condition in conditions)
            {
                var range = GetRangeForConditionAndDifficulty(context, condition, difficulty);
                var value = _random.Next(range.min, range.max + 1);

                if (value > 0)
                {
                    challengeConditions.Add(new ChallengeCondition
                    {
                        Type = condition,
                        Value = value
                    });
                }
            }

            tries++;

            // If we dont get a valid extra conditions after 100 tries we just go with height
            if (tries > 100) break;
        }

        //TODO: Generate Mods
        var mods = await GenerateModsForChallenge(context, difficulty);

        return new Challenge
        {
            Date = DateOnly.FromDateTime(_day.Date),
            Points = (byte)difficulty,
            Mods = mods,
            Conditions = challengeConditions.ToHashSet()
        };
    }

    private async Task<string> GenerateModsForChallenge(TetrioContext context, Difficulty difficulty)
    {
        var mods = await context.Mods.ToListAsync();

        var maxMods = 0;

        switch (difficulty)
        {
            case Difficulty.Easy:
                return string.Empty;
            case Difficulty.Normal:
                maxMods = 2;
                break;
            case Difficulty.Hard:
                maxMods = 3;
                break;
            default:
                return string.Empty;
                break;
        }

        var selectedMods = new List<Mod>();

        var modCountProbability = new byte[1000];

        for (var i = 0; i < modCountProbability.Length; i++)
        {
            modCountProbability[i] = (byte) _random.Next(0, maxMods + 1);
        }

        var modCount = modCountProbability.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        var totalWeight = 0;
        var maxWeight = 100;

        var rand = new Random();

        var tries = 0;

        while (selectedMods.Count < modCount && totalWeight < maxWeight)
        {
            var mod = mods[rand.Next(mods.Count)];

            // If the mod is already in the list we skip it again
            if (selectedMods.Contains(mod)) continue;
            // If adding the mod exceeds the weight limit, skip it
            if (totalWeight + mod.Weight > maxWeight) continue;
            // If the difficulty is lower than the allowed mod we can not add it
            if(difficulty < mod.MinDifficulty) continue;

            selectedMods.Add(mod);
            totalWeight += mod.Weight;

            if (selectedMods.Count >= modCount)
                break;

            tries++;

            if (tries > 150)
                break;

        }

        return string.Join(" ", selectedMods.Select(x => x.Name));
    }

    private static (int min, int max) GetRangeForConditionAndDifficulty(TetrioContext context, ConditionType condition, Difficulty difficulty)
    {
        var range = context.ConditionRanges.FirstOrDefault(x => x.ConditionType == condition && x.Difficulty == difficulty);

        if(range == null)
            return (0, 0);

        return (range.Min, range.Max);
    }

    private List<ConditionType> GetRandomConditions()
    {
        var allConditions = Enum.GetValues<ConditionType>().ToList();

        allConditions.RemoveAt(0);

        allConditions = allConditions.OrderBy(x => _random.Next()).ToList();

        var selectedConditions = allConditions.Take(_random.Next(2, 4)).ToList();

        return selectedConditions;
    }
}