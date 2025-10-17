using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Entities;
using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Zenith.DailyChallenge.ChallengeGeneration.Daily;

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

    public ChallengeGenerator(int seed)
    {
        _day = DateTime.UtcNow;

        _random = new(seed);
    }

    public async Task<List<Challenge>> GenerateChallengesForDay(TetrioContext ctx)
    {
        var dailyGenerator = new DailyChallengeGeneator(_random, _day);
        var reverseGenerator = new ReverseChallengeGenerator(_random, _day);

        List<Challenge> challenges =
        [
            await dailyGenerator.GenerateChallenge(Difficulty.Easy, ctx),
            await dailyGenerator.GenerateChallenge(Difficulty.Normal, ctx),
            await dailyGenerator.GenerateChallenge(Difficulty.Hard, ctx),
            await dailyGenerator.GenerateChallenge(Difficulty.Expert, ctx),
            await reverseGenerator.GenerateReverseChallenge(ctx)
        ];

        return challenges;
    }

    public async Task<MasteryChallenge> GenerateMasteryChallengesForDay(TetrioContext ctx)
    {
        var dailyGenerator = new DailyChallengeGeneator(_random, _day);

        return await dailyGenerator.GenerateMasteryChallenge(ctx);
    }
}