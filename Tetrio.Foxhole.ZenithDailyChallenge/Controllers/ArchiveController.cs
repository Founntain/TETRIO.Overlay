using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Network.Api.Tetrio;

namespace Tetrio.Zenith.DailyChallenge;

public class ArchiveController(TetrioApi api, TetrioContext context) : MinBaseController(api)
{
    [HttpGet]
    [Route("community")]
    public async Task<ActionResult> GetPastCommunityChallenges(Guid? id)
    {
        var baseQuery = context.CommunityChallenges.AsNoTracking().Where(x => x.EndDate < DateTime.UtcNow);

        if (id != null)
        {
            baseQuery = baseQuery.Where(x => x.Id == id);
        }

        var communityChallenge = await baseQuery
            .OrderByDescending(x => x.StartDate)
            .Select(x => new { x.Id, x.StartDate, x.EndDate, x.Value, x.TargetValue, x.ConditionType })
            .FirstOrDefaultAsync();

        if (communityChallenge == null) return NotFound();

        var challengeId = communityChallenge.Id;

        var groupedContributions = await context.CommunityContributions
            .AsNoTracking()
            .Where(x => x.CommunityChallengeId == challengeId && !x.IsLate)
            .GroupBy(x => new { x.CommunityChallengeId, x.UserId, x.User.Username })
            .Select(g => new
            {
                ChallengeId = g.Key.CommunityChallengeId,
                Name = g.Key.Username,
                Contributions = Math.Round(g.Sum(x => x.Amount), 2)
            }).ToListAsync();

        var previousChallenge = context.CommunityChallenges.AsNoTracking().Where(x => x.StartDate < communityChallenge.StartDate).OrderByDescending(x => x.StartDate).FirstOrDefault();
        var nextChallenge = context.CommunityChallenges.AsNoTracking().Where(x => x.StartDate > communityChallenge.StartDate && x.EndDate < DateTime.UtcNow).OrderBy(x => x.StartDate).FirstOrDefault();

        var archiveData = new
        {
            CommunityChallengeId = communityChallenge.Id,
            PreviousChallengeId = previousChallenge?.Id,
            NextChallengeId = nextChallenge?.Id == communityChallenge.Id ? null : nextChallenge?.Id,
            StartDate = communityChallenge.StartDate.ToLongDateString(),
            EndDate = communityChallenge.EndDate.ToLongDateString(),
            Value = Math.Round(communityChallenge.Value, 2),
            TargetValue = Math.Round(communityChallenge.TargetValue, 2),
            ConditionType = communityChallenge.ConditionType,
            Participants = groupedContributions
                .OrderByDescending(y => y.Contributions)
                .Select(y => new { y.Name, y.Contributions })
                .ToArray()
        };

        return Ok(archiveData);
    }
}