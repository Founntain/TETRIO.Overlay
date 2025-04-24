using Microsoft.EntityFrameworkCore;
using Tetrio.Overlay.Database.Entities;
using Tetrio.Overlay.Database.Enums;

namespace Tetrio.Overlay.Database;

public class ChallengeGenerator
{
    private readonly Random _random;
    private readonly DateTime _day;

    public ChallengeGenerator()
    {
        _day = DateTime.UtcNow;

        var seed = int.Parse(_day.ToString("yyyyMMdd"));

        _random = new(seed);
    }

    public async Task<List<Challenge>> GenerateChallengesForDay(TetrioContext context)
    {
        List<Challenge> challenges =
        [
            await GenerateChallenge(Difficulty.Easy, context),
            await GenerateChallenge(Difficulty.Normal, context),
            await GenerateChallenge(Difficulty.Hard, context),
            await GenerateChallenge(Difficulty.Expert, context),
            await GenerateReverseChallenge(context)
        ];

        return challenges;
    }

    private async Task<Challenge> GenerateReverseChallenge(TetrioContext context)
    {
        var challengeConditions = new List<ChallengeCondition>();
        var height = 150;
        var yesterdayChallenge = await context.Challenges.Where(x => x.Points == (byte)Difficulty.Reverse).OrderByDescending(x => x.Date).FirstOrDefaultAsync();
        var randomMod = GetRandomReverseMod(yesterdayChallenge?.Mods);;

        height += randomMod.HeightModifier;

        challengeConditions.Add(new () { Type = ConditionType.Height, Value = height});

        return new Challenge
        {
            Date = DateOnly.FromDateTime(_day.Date),
            Points = (byte) Difficulty.Reverse,
            Mods = randomMod.Mod,
            Conditions = challengeConditions.ToHashSet()
        };
    }

    private (string Mod, int HeightModifier) GetRandomReverseMod(string? challenge)
    {
        var yesterdaysReverseMod = challenge;

        (string, int)? selectedMod = null;

        var tries = 0;

        // If after 300 tries we still don't find a mod that is different from yesterday's one, we just use the one rolled last
        while (tries <= 300)
        {
            var mod = _random.Next(0, 8);

            switch (mod)
            {
                case 0:
                    selectedMod = ("expert_reversed", _random.Next(0, 50)); break;
                case 1:
                    selectedMod = ("nohold_reversed", _random.Next(0, 150)); break;
                case 2:
                    selectedMod = ("messy_reversed", _random.Next(0, 200)); break;
                case 3:
                    selectedMod = ("gravity_reversed", _random.Next(0, 200)); break;
                case 4:
                    selectedMod = ("volatile_reversed", _random.Next(0, 400)); break;
                case 5:
                    selectedMod = ("doublehole_reversed", _random.Next(0, 100)); break;
                case 6:
                    selectedMod = ("invisible_reversed", _random.Next(0, 50)); break;
                case 7:
                    selectedMod = ("allspin_reversed", _random.Next(0, 200)); break;
                // We default to reverse volatile, as it is the easiest for most.
                // However, the default case should never trigger.
                default: selectedMod = ("volatile_reversed", _random.Next(0, 400)); break;
            }

            // We generate mods until we find a reverse mod that is different from yesterdays one
            if (selectedMod.Value.Item1 != yesterdaysReverseMod) break;

            tries++;
        }

        // Just as a fallback, if selectedMod is still null, we just use the default case.
        selectedMod ??= ("volatile_reversed", _random.Next(0, 400));

        return selectedMod.Value;
    }

    private async Task<Challenge> GenerateChallenge(Difficulty difficulty, TetrioContext context)
    {
        var challengeConditions = new List<ChallengeCondition>();

        // Always add Height as a base condition
        var heightRange = GetRangeForConditionAndDifficulty(context, ConditionType.Height, difficulty);
        var height = _random.Next((int)heightRange.min, (int)heightRange.max + 1);

        var mods = await GenerateModsForChallenge(context, difficulty);

        var heightScaling = 1d;

        foreach (var mod in mods.Split(" "))
        {
            if (mod == "nohold")
            {
                if(heightScaling > 0.9) heightScaling = 0.9;
            }

            if (difficulty != Difficulty.Expert && mod == "expert")
            {
                if (heightScaling > 0.75) heightScaling = 0.75;
            }
        }


        height = (int) Math.Round(height * heightScaling, 0);

        challengeConditions.Add(new () { Type = ConditionType.Height, Value = height});

        var tries = 0;

        while (challengeConditions.Count == 1)
        {
            var conditions = GetRandomConditions(mods);

            foreach (var condition in conditions)
            {
                var range = GetRangeForConditionAndDifficulty(context, condition, difficulty);
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

        while (selectedMods.Count < modCount && totalWeight < maxWeight)
        {
            var mod = mods[rand.Next(mods.Count)];

            // If the difficulty is lower than the allowed mod we can not add it
            if(difficulty < mod.MinDifficulty) continue;
            // If the mod is already in the list we skip it again
            if (selectedMods.Contains(mod)) continue;
            // If adding the mod exceeds the weight limit, skip it
            if (totalWeight + mod.Weight > maxWeight) continue;

            if(mod.Name == "expert") continue;

            selectedMods.Add(mod);
            totalWeight += mod.Weight;

            if (selectedMods.Count >= modCount)
                break;

            tries++;

            // If we don't get a valid extra conditions after 150 tries we just go with no mods at all or what we have already
            if (tries > 150)
                break;
        }

        return string.Join(" ", selectedMods.Select(x => x.Name));
    }

    private static (double min, double max) GetRangeForConditionAndDifficulty(TetrioContext context, ConditionType condition, Difficulty difficulty)
    {
        var range = context.ConditionRanges.FirstOrDefault(x => x.ConditionType == condition && x.Difficulty == difficulty);

        if(range == null)
            return (0, 0);

        return (range.Min, range.Max);
    }

    private List<ConditionType> GetRandomConditions(string mods)
    {
        var allConditions = Enum.GetValues<ConditionType>().ToList();

        allConditions.RemoveAt(0);

        // If no hold was selected as mod, remove all clears from the condition roll
        if(mods.Contains("nohold")) allConditions.Remove(ConditionType.AllClears);

        allConditions = allConditions.OrderBy(_ => _random.Next()).ToList();

        var selectedConditions = allConditions.Take(_random.Next(2, 4)).ToList();

        return selectedConditions;
    }
}