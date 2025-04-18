using Microsoft.EntityFrameworkCore;
using Tetrio.Overlay.Database.Entities;
using Tetrio.Overlay.Database.Enums;

namespace Tetrio.Overlay.Database;

public class CommunityChallengeGenerator
{
    private readonly Random _random;
    private readonly DateTime _day;

    public CommunityChallengeGenerator()
    {
        _day = DateTime.Now;

        var seed = int.Parse(_day.ToString("yyyyMMdd"));

        _random = new(seed);
    }

    public async Task<CommunityChallenge> GenerateCommunityChallenge(TetrioContext context)
    {
        var communityChallenge = new CommunityChallenge();

        var allConditions = Enum.GetValues<ConditionType>().ToList();
        allConditions = allConditions.OrderBy(_ => _random.Next()).ToList();

        ConditionType? selectedCondition = null;

        while (selectedCondition == null || selectedCondition == ConditionType.Finesse)
        {
            selectedCondition = allConditions.OrderBy(_ => _random.Next()).FirstOrDefault();
        }

        var conditionValues = await context.ConditionRanges.Where(x => x.Difficulty == Difficulty.Community && x.ConditionType == selectedCondition).FirstAsync();

        var targetValue = _random.Next((int) conditionValues.Min, (int) conditionValues.Max);

        targetValue = (targetValue / 1000) * 1000;

        communityChallenge.ConditionType = selectedCondition.Value;
        communityChallenge.TargetValue = targetValue;
        communityChallenge.StartDate = new DateTime(_day.Year, _day.Month, _day.Day, 0 , 0 , 0);
        communityChallenge.EndDate = communityChallenge.StartDate.AddDays(7).AddSeconds(-1);

        return communityChallenge;
    }
}