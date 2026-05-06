using System.Globalization;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Entities;
using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Zenith.DailyChallenge.Controllers;

public class WeeklyChallengeGenerator
{
    private readonly Random _random;
    private readonly DateOnly _day;

    public WeeklyChallengeGenerator(DateOnly startDate)
    {
        _day = startDate;

        var seed = int.Parse(_day.ToString("yyyyMMdd"));

        _random = new(seed);
    }

    public async Task<WeeklyChallenge> Generate(TetrioContext ctx)
    {
        var conditionTypes = new List<WeeklyConditionType>() { WeeklyConditionType.Height };

        conditionTypes.AddRange(GetRandomConditions());

        var challengeConditions = new List<WeeklyChallengeCondition>();

        foreach (var condition in conditionTypes)
        {
            var balancing = WeeklyBalancing.Balancing[condition];
            var value = _random.Next((int)balancing.Min, (int)balancing.Max + 1);

            challengeConditions.Add(new ()
            {
                Type = condition,
                Value = value
            });
        }

        // calculate calender week of the day
        var week = (byte) ISOWeek.GetWeekOfYear(_day.ToDateTime(TimeOnly.MinValue));

        var weeklyChallenge = new WeeklyChallenge()
        {
            StartDate = _day,
            Conditions = challengeConditions.ToHashSet(),
            Week = week
        };

        await ctx.WeeklyChallenges.AddAsync(weeklyChallenge);
        await ctx.SaveChangesAsync();

        return weeklyChallenge;
    }

    private List<WeeklyConditionType> GetRandomConditions()
    {
        var conditionAmount = _random.Next(1, 5);
        var allConditions = Enum.GetValues<WeeklyConditionType>().ToList();

        allConditions.RemoveAt(0);
        // Removed finesse for challenge generator because of popular request

        allConditions = allConditions.OrderBy(_ => _random.Next()).ToList();

        var selectedConditions = allConditions.Take(conditionAmount).ToList();

        return selectedConditions;
    }
}