using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Network.Api.Tetrio;

namespace Tetrio.Zenith.DailyChallenge.Controllers;

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
            .Select(x => new { x.Id, x.StartDate, x.EndDate, x.Value, x.TargetValue, x.ConditionType, x.Name, x.Description, x.Mods })
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
        var nextChallenge = context.CommunityChallenges.AsNoTracking().Where(x => x.StartDate > communityChallenge.StartDate && x.EndDate < DateTime.UtcNow && x.Id != communityChallenge.Id).OrderBy(x => x.StartDate).FirstOrDefault();

        var archiveData = new
        {
            CommunityChallengeId = communityChallenge.Id,
            PreviousChallengeId = previousChallenge?.Id,
            NextChallengeId = nextChallenge?.Id,
            StartDate = communityChallenge.StartDate.ToLongDateString(),
            EndDate = communityChallenge.EndDate.ToLongDateString(),
            Name = communityChallenge.Name,
            Description = communityChallenge.Description,
            Value = Math.Round(communityChallenge.Value, 2),
            Mods = communityChallenge.Mods?.Split(" ", StringSplitOptions.RemoveEmptyEntries),

            TargetValue = Math.Round(communityChallenge.TargetValue, 2),
            ConditionType = communityChallenge.ConditionType,
            Participants = groupedContributions
                .OrderByDescending(y => y.Contributions)
                .Select(y => new { y.Name, y.Contributions })
                .ToArray()
        };

        return Ok(archiveData);
    }

    [HttpGet]
    [Route("daily")]
    public async Task<ActionResult> GetPastDailyChallenges(DateOnly? date = null)
    {
        if (date == null)
        {
            // If no date is provided, find the most recent date with challenges
            var latest = await context.Challenges.AsNoTracking()
                .OrderByDescending(x => x.Date)
                .Select(x => new { x.Date })
                .FirstOrDefaultAsync();

            if (latest == null) return NotFound();

            date = latest.Date;
        }

        var minDate = await context.Challenges.AsNoTracking().MinAsync(x => x.Date);
        var maxDate = await context.Challenges.AsNoTracking().MaxAsync(x => x.Date);

        var rawData = await context.Challenges.AsNoTracking()
            .Where(x => x.Date == date)
            .OrderBy(x => x.Points)
            .Select(x => new
            {
                x.Date,
                x.Points,
                x.Mods,
                Conditions = x.Conditions.Select(y => new { y.Type, y.Value }),
                Runs = x.Runs.Select(r => new { r.User.Username, r.PlayedAt })
            }).ToListAsync();

        if (rawData.Count == 0) return NotFound();

        var archiveData = rawData.Select(x => new
        {
            MinDate = minDate,
            MaxDate = maxDate,
            Date = x.Date.ToString("D"),
            Points = x.Points,
            Mods = x.Mods?.Split(" ", StringSplitOptions.RemoveEmptyEntries),
            Conditions = x.Conditions.OrderBy ( x=> x.Type),
            Users = x.Runs
                .GroupBy(r => r.Username)
                .Select(g => new
                {
                    Username = g.Key,
                    CompletedAt = g.Select(r => r.PlayedAt).Min()?.ToString("HH:m:s")
                }).OrderBy(y => y.CompletedAt)
        });

        return Ok(archiveData);
    }
}