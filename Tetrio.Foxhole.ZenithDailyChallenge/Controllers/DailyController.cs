using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Entities;
using Tetrio.Foxhole.Database.Enums;
using Tetrio.Foxhole.Network.Api.Tetrio;

namespace Tetrio.Zenith.DailyChallenge.Controllers;

[Route("zenith/daily")]
public class DailyController(TetrioApi api, TetrioContext context) : BaseController(api)
{
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetDailyChallenges(ulong discordId = 0)
    {
        var day = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);

        var challengeCount = await context.Challenges.AsNoTracking().Where(x => x.Date == day).CountAsync();
        var masteryChallengeCount = await context.MasteryChallenges.AsNoTracking().Where(x => x.Date == day).CountAsync();

        if (challengeCount == 0 || masteryChallengeCount == 0)
        {
            await GenerateDailyChallenges();
        }

        var challenges = await context.Challenges.AsNoTracking().Where(x => x.Date == day).OrderByDescending(x => x.Points).Select(x => new
        {
            Id = x.Id,
            IsMasteryChallenge = false,
            Conditions = x.Conditions.OrderBy(y => y.Type).Select(y => new ChallengeCondition
            {
                Id = y.Id,
                ChallengeId = y.ChallengeId,
                Value = y.Value,
                Type = y.Type,
            }).ToHashSet(),
            Date = x.Date,
            Points = x.Points,
            Mods = x.Mods,
        }).ToListAsync();

        var masteryChallenge = await context.MasteryChallenges.AsNoTracking().Where(x => x.Date == day).Select(x => new
        {
            Id = x.Id,
            IsMasteryChallenge = true,
            Conditions = x.Conditions.OrderBy(y => y.Type).Select(y => new MasteryChallengeCondition()
            {
                Id = y.Id,
                ChallengeId = y.ChallengeId,
                Value = y.Value,
                Type = y.Type,
            }).ToHashSet(),
            Date = x.Date
        }).FirstOrDefaultAsync();

        var allChallenges = new List<object>();

        allChallenges.AddRange(challenges);

        if(masteryChallenge != null)
            allChallenges.AddRange(masteryChallenge);

        return Ok(allChallenges);
    }

    [HttpGet]
    [Route("date")]
    public ActionResult GetDate()
    {
        var date = DateTime.UtcNow.Date;
        var runsUntil = date.AddDays(1).AddSeconds(-1);

        return Ok(new
        {
            DateString = date.ToString("dddd, dd. MMMM yyyy"),
            Date = date,
            DateUnixSeconds = ((DateTimeOffset)date).ToUnixTimeSeconds(),
            RunsUntil = runsUntil,
            RunsUntilUnixSeconds = ((DateTimeOffset)runsUntil).ToUnixTimeSeconds()
        });
    }

    [HttpGet]
    [Route("generate")]
    public async Task<IActionResult> GenerateDailyChallenges()
    {
        var generator = new ChallengeGenerator();

        var day = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);

        var challengesExist = await context.Challenges.AsNoTracking().AnyAsync(x => x.Date == day);
        var masteryChallengeExists = await context.MasteryChallenges.AsNoTracking().AnyAsync(x => x.Date == day);

        if (challengesExist && masteryChallengeExists)
        {
            return Ok("Challenges already exist for this day.");
        }

        await using var transaction = await context.Database.BeginTransactionAsync();

        if (!challengesExist)
        {
            var challenges = await generator.GenerateChallengesForDay(context);
            await context.AddRangeAsync(challenges);
        }

        if (!masteryChallengeExists)
        {
            var masteryChallenge = await generator.GenerateMasteryChallenge(context);
            await context.AddAsync(masteryChallenge);
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok();
    }

    [HttpPost]
    [Route("submit")]
    public async Task<IActionResult> SubmitDailyChallenge()
    {
        var authResult = await CheckIfAuthorized(context);

        if (!authResult.IsAuthorized)
        {
            ResetCookies();

            return StatusCode(authResult.StatusCode, $"{authResult.StatusCode} - Unauthorized. Reason: {authResult.ResponseText}");
        }

        var user = authResult.User;

        if (user == null) return Ok("You are not authorized to submit daily challenges, please log in again and try again");
        if (user.IsRestricted) return BadRequest("No bad person, no submitting for you, ask founntain to unrestrict you");

        var day = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var submitLogic = new SubmitLogic(context, Api, user, day);

        var response = await submitLogic.ProcessSubmissions();

        return response.ResponseCode != 200 ? StatusCode(response.ResponseCode, response.ResultObject) : Ok(response.ResultObject);
    }

    [HttpGet]
    [Route("getLeaderboard")]
    public async Task<IActionResult> GetLeaderboard(int page = 1, int pageSize = 30)
    {
        var users = await context.Users.AsNoTracking().Where(x => x.Challenges.Count > 0).Select(x => new
            {
                User = new
                {
                    Name = x.Username
                },
                NormalScore = x.Challenges.Where(y => y.Points != (byte)Difficulty.Expert && y.Points != (byte)Difficulty.Reverse).Sum(y => y.Points),
                ExpertScore = x.Challenges.Where(y => y.Points == (byte)Difficulty.Expert).Sum(y => y.Points),
                ReverseScore = x.Challenges.Where(y => y.Points == (byte)Difficulty.Reverse).Sum(y => y.Points),
                EasyChallenges = x.Challenges.Count(y => y.Points == (byte)Difficulty.Easy),
                NormalChallenges = x.Challenges.Count(y => y.Points == (byte)Difficulty.Normal),
                HardChallenges = x.Challenges.Count(y => y.Points == (byte)Difficulty.Hard),
                ExpertChallengesCompleted = x.Challenges.Count(y => y.Points == (byte)Difficulty.Expert),
                ReverseChallengesCompleted = x.Challenges.Count(y => y.Points == (byte)Difficulty.Reverse),
                MasteryScore = x.MasteryAttempts.Select(y => new
                    {
                        MasteryChallengeModsCompleted = (y.ExpertCompleted ? 1 : 0) +
                                                        (y.NoHoldCompleted ? 1 : 0) +
                                                        (y.MessyCompleted ? 1 : 0) +
                                                        (y.GravityCompleted ? 1 : 0) +
                                                        (y.VolatileCompleted ? 1 : 0) +
                                                        (y.DoubleHoleCompleted ? 1 : 0) +
                                                        (y.InvisibleCompleted ? 1 : 0) +
                                                        (y.AllSpinCompleted ? 1 : 0)

                    }).Sum(y => y.MasteryChallengeModsCompleted)
            }).OrderByDescending(x => x.NormalScore + x.ExpertScore + x.ReverseScore)
                .ThenByDescending( x => (x.EasyChallenges + x.NormalChallenges + x.HardChallenges))
                .ThenByDescending( x => (x.ExpertChallengesCompleted + x.ReverseChallengesCompleted))
                .Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync();

        var validUserCount = await context.Users.AsNoTracking().Where(x => x.Challenges.Count > 0).CountAsync();

        var leaderboardData = users.Select(x => new
            {
                Username = x.User.Name,
                Score = Math.Round(x.NormalScore + x.ExpertScore + ( x.MasteryScore * 2 )  + (x.ReverseScore / 2d), 0),
                EasyChallengesCompleted = x.EasyChallenges,
                NormalChallengesCompleted = x.NormalChallenges,
                HardChallengesCompleted = x.HardChallenges,
                ExpertChallengesCompleted = x.ExpertChallengesCompleted,
                ReverseChallengesCompleted = x.ReverseChallengesCompleted,
                MasteryChallengesCompleted = x.MasteryScore,
            }).OrderByDescending(x => x.Score)
              .ThenByDescending( x => (x.EasyChallengesCompleted + x.NormalChallengesCompleted + x.HardChallengesCompleted))
              .ThenByDescending( x => (x.ExpertChallengesCompleted + x.ReverseChallengesCompleted)).ToArray();

        return Ok(new
        {
            Leaderboard = leaderboardData,
            TotalUsers = validUserCount,
        });
    }

    [HttpGet]
    [Route("getCommunityChallenge")]
    public async Task<IActionResult> GetCommunityChallenge()
    {
        var now = DateTime.UtcNow;

        var communityChallenge = await context.CommunityChallenges.AsNoTracking().Select(x => new
        {
            Id = x.Id,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            ConditionType = x.ConditionType,
            Value = Math.Round(x.Value, 2),
            TargetValue = x.TargetValue,
            Finished = x.Finished,
            TotalContributions = x.Contributions.Count(),
            Participants = x.Contributions.Where(y => !y.IsLate).GroupBy(y => y.UserId).Count()
        }).FirstOrDefaultAsync(x => x.StartDate <= now && x.EndDate >= now);

        if(communityChallenge == null) return Ok(new { Finished = false });

        var topContributers = await context.CommunityContributions.AsNoTracking().Where(x => x.CommunityChallengeId == communityChallenge.Id && x.IsLate == false).GroupBy(x => x.UserId).Select(x => new
        {
            Username = x.First().User.Username,
            Amount = Math.Round(x.Sum(y => y.Amount), 2)
        }).OrderByDescending(x => x.Amount).Where(x => x.Amount > 0).ToArrayAsync();

        return Ok( new
        {
            communityChallenge,
            topContributers,
            StartedAtUnixSeconds = ((DateTimeOffset)DateTime.SpecifyKind(communityChallenge.StartDate, DateTimeKind.Utc)).ToUnixTimeSeconds(),
            EndsAtUnixSeconds = ((DateTimeOffset)DateTime.SpecifyKind(communityChallenge.EndDate, DateTimeKind.Utc)).ToUnixTimeSeconds(),
        });
    }

    [HttpGet]
    [Route("getRecentCommunityContributions")]
    public async Task<IActionResult> GetRecentCommunityContributions()
    {
        var now = DateTime.UtcNow;

        var isCommunityChallengeActive = await context.CommunityChallenges.AsNoTracking().AnyAsync(x => x.StartDate <= now && x.EndDate >= now);

        if(!isCommunityChallengeActive) return Ok(new List<object>());

        var recentContributions = await context.CommunityContributions
            .Where(x => x.CommunityChallenge.StartDate <= now && x.CommunityChallenge.EndDate >= now)
            .OrderByDescending(x => x.CreatedAt).Select(x => new
            {
                Username = x.User.Username,
                Amount = Math.Round(x.Amount,2),
                ConditionType = x.CommunityChallenge.ConditionType,
                IsLate = x.IsLate,
            }).Take(5).ToListAsync();

        return Ok(recentContributions);
    }

#if DEBUG
    [HttpGet]
    [Route("generateTestChallenges")]
    public async Task<IActionResult> GenerateTestChallenges(Difficulty dif, int amount)
    {
        if (amount > 1000) return BadRequest("Amount is not allowed to be bigger than 1000");

        var random = new Random();

        var c = new ChallengeGenerator(random.Next(1, 100000000));

        var challenges = new List<Challenge>();

        for (int i = 0; i < 1000; i++)
        {
            challenges.Add(await c.GenerateChallenge(dif, context));
        }

        var a = challenges.Select(x => new
        {
            c = x.Conditions.Select(y => new
            {
                t =y.Type.ToString(),
                v = y.Value
            }),
            m = x.Mods
        }).ToArray();

        return Ok(a);
    }
#endif
}