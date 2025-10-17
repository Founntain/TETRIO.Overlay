using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Entities;
using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Zenith.DailyChallenge.ChallengeGeneration.Daily;

public class ReverseChallengeGenerator : BaseChallengeGenerator
{
    public ReverseChallengeGenerator(Random random, DateTime day) : base(random, day)
    {

    }

    public async Task<Challenge> GenerateReverseChallenge(TetrioContext ctx)
    {
        var challengeConditions = new List<ChallengeCondition>();
        var height = 150;
        var yesterdayChallenge = await ctx.Challenges.Where(x => x.Points == (byte)Difficulty.Reverse).OrderByDescending(x => x.Date).FirstOrDefaultAsync();
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
            tries++;

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
        }

        // Just as a fallback, if selectedMod is still null, we just use the default case.
        selectedMod ??= ("volatile_reversed", _random.Next(0, 400));

        return selectedMod.Value;
    }
}