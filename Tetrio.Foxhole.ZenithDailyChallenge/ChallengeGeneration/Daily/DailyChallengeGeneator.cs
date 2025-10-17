using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Entities;
using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Zenith.DailyChallenge.ChallengeGeneration.Daily;

public class DailyChallengeGeneator : BaseChallengeGenerator
{

    public DailyChallengeGeneator(Random random, DateTime day) : base(random, day)
    {

    }

    public async Task<MasteryChallenge> GenerateMasteryChallenge(TetrioContext context)
    {
        var conditions = GetRandomConditionsWithoutAllClears();

        var masteryChallengeConditions = new List<MasteryChallengeCondition>();

        // Always add Height as a base condition
        var heightRange = await GetRangeForConditionAndDifficulty(context, ConditionType.Height, Difficulty.Normal);
        var height = _random.Next((int)heightRange.min, (int)heightRange.max + 1);

        masteryChallengeConditions.Add(new () { Type = ConditionType.Height, Value = height});

        foreach (var condition in conditions)
        {
            var range = await GetRangeForConditionAndDifficulty(context, condition, Difficulty.Normal);

            double value;

            if (condition is ConditionType.Pps or ConditionType.Apm or ConditionType.Vs)
            {
                value = range.min + _random.NextDouble() * (range.max - range.min);

                value = Math.Round(value, 2);
            }
            else
            {
                value = _random.Next((int) range.min, (int) range.max);
            }

            if (value > 0)
            {
                var masteryChallengeCondition = new MasteryChallengeCondition()
                {
                    Type = condition,
                    Value = value
                };

                masteryChallengeConditions.Add(masteryChallengeCondition);
            }
        }

        return new MasteryChallenge
        {
            Date = DateOnly.FromDateTime(_day.Date),
            Conditions = masteryChallengeConditions.ToHashSet(),
        };
    }

    public async Task<Challenge> GenerateChallenge(Difficulty difficulty, TetrioContext context)
    {
        var challengeConditions = new List<ChallengeCondition>();

        // Always add Height as a base condition
        var heightRange = await GetRangeForConditionAndDifficulty(context, ConditionType.Height, difficulty);
        var height = _random.Next((int)heightRange.min, (int)heightRange.max + 1);

        var mods = await GenerateModsForChallenge(context, difficulty);

        var scalingFactors = CalculateNerfAdjustmentFactors(mods.Split(' '));

        height = (int) Math.Round(height * scalingFactors.Alitude, 0);

        challengeConditions.Add(new () { Type = ConditionType.Height, Value = height});

        var tries = 0;

        while (challengeConditions.Count == 1)
        {
            tries++;

            // If we dont get valid extra conditions after 100 tries we just go without them
            if (tries > 100) break;

            var conditions = GetRandomConditions(mods);

            foreach (var condition in conditions)
            {
                var range = await GetRangeForConditionAndDifficulty(context, condition, difficulty);
                double value;

                if (condition is ConditionType.Pps or ConditionType.Apm or ConditionType.Vs)
                {
                    value = range.min + _random.NextDouble() * (range.max - range.min);

                    value = Math.Round(value, 2);
                }
                else
                {
                    value = _random.Next((int) range.min, (int) range.max);
                }

                value = condition switch
                {
                    // Apply scaling factors if needed
                    ConditionType.Apm => Math.Round(value * scalingFactors.Apm, 2),
                    ConditionType.Vs => Math.Round(value * scalingFactors.Vs, 2),
                    _ => value
                };

                // Make sure the value is within the range, it cant be lower than the minimum
                if (value < range.min) value = range.min;

                if (value > 0)
                {
                    challengeConditions.Add(new ChallengeCondition
                    {
                        Type = condition,
                        Value = value
                    });
                }
            }
        }

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
        var modCountProbability = new byte[1000];
        var selectedMods = new List<Mod>();
        var mods = await context.Mods.ToListAsync();

        mods = mods.OrderBy(_ => _random.NextInt64()).ToList();

        int maxMods;

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
            case Difficulty.Expert:
                maxMods = 1;
                selectedMods.Add(mods.First(x => x.Name == "expert"));
                break;
            default:
                return string.Empty;
        }

        for (var i = 0; i < modCountProbability.Length; i++)
        {
            modCountProbability[i] = (byte) _random.Next(0, maxMods + 1);
        }

        var modCount = modCountProbability.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        var totalWeight = 0;
        var maxWeight = 100;

        var rand = new Random();

        var tries = 0;

        var pastDaysMods = new List<string>();

        if (difficulty == Difficulty.Hard)
        {
            pastDaysMods = (await context.Challenges
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .Where(x => x.Points == (byte)Difficulty.Hard)
                    .Take(1).ToArrayAsync())
                .SelectMany(x => x.Mods.Split(" ")).ToList();
        }

        while (selectedMods.Count < modCount && totalWeight < maxWeight)
        {
            tries++;

            // If we don't get a valid extra conditions after 150 tries we just go with no mods at all or what we have already
            if (tries > 150)
                break;

            var mod = mods[rand.Next(mods.Count)];

            // If the difficulty is lower than the allowed mod we can not add it
            if(difficulty < mod.MinDifficulty) continue;
            // If the mod is already in the list we skip it again
            if (selectedMods.Contains(mod)) continue;
            // If the mod was part of the past 3 days we skip it
            if(pastDaysMods.Count > 0 && pastDaysMods.Contains(mod.Name)) continue;
            // If adding the mod exceeds the weight limit, skip it
            if (totalWeight + mod.Weight > maxWeight) continue;

            if(mod.Name == "expert") continue;

            selectedMods.Add(mod);
            totalWeight += mod.Weight;

            if (selectedMods.Count >= modCount)
                break;
        }

        return string.Join(" ", selectedMods.Select(x => x.Name));
    }

}