using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Enums;
using Tetrio.Foxhole.Network.Api.Tetrio;

namespace Tetrio.Zenith.DailyChallenge.Controllers;

public class LeaderboardController(TetrioApi api, TetrioContext context) : BaseController(api)
{
    [HttpGet]
    public async Task<IActionResult> GetLeaderboard(DateTime? date = null)
    {
        var leaderboardDate = date.HasValue
            ? DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
            : DateTime.UtcNow;

        var leaderboard = await context.Leaderboards.AsNoTracking()
            .Where(x => x.StartDate <= leaderboardDate && (x.EndDate == null || x.EndDate >= leaderboardDate))
            .Select(x => new
            {
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Name = x.Name,
                Description = x.Description,
                UserIds = x.Entries
                    .Where(y => y.Score > 0)
                    .OrderByDescending(y => y.Score)
                    .Select(y => y.UserId)
                    .ToList(),
                Entries = x.Entries
                    .Where(y => y.Score > 0)
                    .OrderByDescending(y => y.Score)
                    .Select(y => new
                    {
                        Rank = y.User.TetrioRank,
                        Username = y.User.Username,
                        UserId = y.User.Id,
                        Score = y.Score,
                    }).ToList()
            }).FirstOrDefaultAsync();

        if (leaderboard == null) return NotFound($"No leaderboard found for timestamp {leaderboardDate}");

        var runStats = await context.Runs.AsNoTracking()
            .Where(x => x.PlayedAt >= leaderboard.StartDate && x.PlayedAt <= leaderboard.EndDate && leaderboard.UserIds.Contains(x.UserId))
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TopRun = g.Max(y => y.Altitude),
                Apm = g.Average(y => y.Apm),
                Vs = g.Average(y => y.Vs),
                Kos = g.Sum(y => y.KOs),
                Runs = g.Count()
            }).ToDictionaryAsync(x => x.UserId);

        var userLevels = await context.UserXps.AsNoTracking()
            .Where(x => x.Type == XpType.Lifetime && leaderboard.UserIds.Contains(x.User.Id))
            .GroupBy(x => x.User.Id)
            .Select(g => new
            {
                UserId = g.Key,
                Level = g.FirstOrDefault().CalculateLevel()
            }).ToDictionaryAsync(x => x.UserId);

        var leaderboardData = leaderboard.Entries.Select(x => new
        {
            x.Rank,
            x.Username,
            x.Score,
            TopRun = runStats.TryGetValue(x.UserId, out var stats) ? stats.TopRun : 0,
            Apm = stats?.Apm ?? 0,
            Vs = stats?.Vs ?? 0,
            Kos = stats?.Kos ?? 0,
            Runs = stats?.Runs ?? 0,
            Level = userLevels.TryGetValue(x.UserId, out var level) ? level.Level : 0
        });

        return Ok(new
        {
            leaderboard.StartDate,
            leaderboard.EndDate,
            StartedAtUnixSeconds = ((DateTimeOffset)DateTime.SpecifyKind(leaderboard.StartDate, DateTimeKind.Utc)).ToUnixTimeSeconds(),
            EndsAtUnixSeconds = leaderboard.EndDate.HasValue ? ((DateTimeOffset)DateTime.SpecifyKind(leaderboard.EndDate.Value, DateTimeKind.Utc)).ToUnixTimeSeconds() : -1,
            leaderboard.Name,
            leaderboard.Description,
            Leaderboard = leaderboardData
        });
    }

    [HttpGet]
    [Route("info")]
    public async Task<IActionResult> GetLeaderboardInfo(DateTime? date = null)
    {
        var leaderboardDate = date.HasValue
            ? DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
            : DateTime.UtcNow;

        var leaderboard = await context.Leaderboards.AsNoTracking()
            .Where(x => x.StartDate <= leaderboardDate && (x.EndDate == null || x.EndDate >= leaderboardDate))
            .Select(x => new
            {
                Id = x.Id,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Name = x.Name,
                Description = x.Description,
                Leaderboard = x.Entries.Select(y => new
                {
                    Rank = y.User.TetrioRank,
                    Username = y.User.Username,
                    UserId = y.User.Id,
                    Score = y.Score
                }).Where(y => y.Score > 0).OrderByDescending(y => y.Score).ToList()
            }).FirstOrDefaultAsync();

        if (leaderboard == null) return NotFound($"No leaderboard found for timestamp {leaderboardDate}");

        var participants = await context.LeaderboardEntries.AsNoTracking().CountAsync(x => x.LeaderboardId == leaderboard.Id);

        var leaderboardData = leaderboard.Leaderboard.Select(x => new
        {
            x.Rank,
            x.Username,
            x.Score,
            Level = (context.UserXps.AsNoTracking().FirstOrDefault(y => y.Type == XpType.Lifetime && y.User.Id == x.UserId))?.CalculateLevel() ?? 0
        }).Take(3);

        return Ok(new
        {
            leaderboard.StartDate,
            leaderboard.EndDate,
            StartedAtUnixSeconds = ((DateTimeOffset)DateTime.SpecifyKind(leaderboard.StartDate, DateTimeKind.Utc)).ToUnixTimeSeconds(),
            EndsAtUnixSeconds = leaderboard.EndDate.HasValue ? ((DateTimeOffset)DateTime.SpecifyKind(leaderboard.EndDate.Value, DateTimeKind.Utc)).ToUnixTimeSeconds() : -1,
            leaderboard.Name,
            leaderboard.Description,
            participants,
            Leaderboard = leaderboardData
        });
    }

    [HttpGet]
    [Route("{username}")]
    public async Task<IActionResult> GetLeaderboardPosition(string username, DateTime? date = null)
    {
        username = username.ToLower();

        var user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);

        if(user == null) return NotFound($"User '{username}' not found");

        var leaderboardDate = date ?? DateTime.UtcNow;

        var leaderboard = (await context.Leaderboards
            .Select(x => new {x.StartDate, x.EndDate, x.Id, x.Name})
            .FirstOrDefaultAsync(x => x.StartDate <= leaderboardDate && (x.EndDate == null || x.EndDate >= leaderboardDate)));

        if (leaderboard == null) return NotFound($"Leaderboard not found for timestamp {leaderboardDate} not found");

        var userEntry = await context.LeaderboardEntries.FirstOrDefaultAsync(x => x.Leaderboard.Id == leaderboard.Id && x.User.Id == user.Id);

        if (userEntry == null) return NotFound("User is not placed on this leaderboard");

        var position = await context.LeaderboardEntries.CountAsync(x => x.Leaderboard.Id == leaderboard.Id && x.Score > userEntry.Score) + 1;

        return Ok(new
        {
            Placement = position,
            SeasonName = leaderboard.Name
        });
    }

    [HttpGet]
    [Route("getGlobalLeaderboard")]
    public async Task<IActionResult> GetGlobalLeaderboard(int page = 1, int pageSize = 30)
    {
        var users = await context.Users.AsNoTracking().Where(x => x.Challenges.Count > 0).Select(x => new
            {
                User = new
                {
                    Name = x.Username,
                    Score = x.Score,
                    Rank = x.TetrioRank ?? "z",
                    Level = x.Xp.FirstOrDefault(x => x.Type == XpType.Lifetime).CalculateLevel()
                },
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
                                                        (y.AllSpinCompleted ? 1 : 0) +
                                                        (y.ExpertReversedCompleted ? 1 : 0) +
                                                        (y.NoHoldReversedCompleted ? 1 : 0) +
                                                        (y.MessyReversedCompleted ? 1 : 0) +
                                                        (y.GravityReversedCompleted ? 1 : 0) +
                                                        (y.VolatileReversedCompleted ? 1 : 0) +
                                                        (y.DoubleHoleReversedCompleted ? 1 : 0) +
                                                        (y.InvisibleReversedCompleted ? 1 : 0) +
                                                        (y.AllSpinReversedCompleted ? 1 : 0)

                    }).Sum(y => y.MasteryChallengeModsCompleted)
            }).OrderByDescending(x => x.User.Score)
                .ThenByDescending( x => (x.EasyChallenges + x.NormalChallenges + x.HardChallenges))
                .ThenByDescending( x => (x.ExpertChallengesCompleted + x.ReverseChallengesCompleted))
                .Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync();

        var validUserCount = await context.Users.AsNoTracking().Where(x => x.Challenges.Count > 0).CountAsync();

        var leaderboardData = users.Select(x => new
        {
            Username = x.User.Name,
            Rank = x.User.Rank,
            Score = x.User.Score,
            Level = x.User.Level,
            EasyChallengesCompleted = x.EasyChallenges,
            NormalChallengesCompleted = x.NormalChallenges,
            HardChallengesCompleted = x.HardChallenges,
            ExpertChallengesCompleted = x.ExpertChallengesCompleted,
            ReverseChallengesCompleted = x.ReverseChallengesCompleted,
            MasteryChallengesCompleted = x.MasteryScore,
        });

        return Ok(new
        {
            Leaderboard = leaderboardData,
            TotalUsers = validUserCount,
        });
    }

    [HttpGet]
    [Route("getLegacyLeaderboard")]
    public async Task<IActionResult> GetLegacyLeaderboard()
    {
        var users = await context.Users.AsNoTracking().Where(x => x.LegacyScore > 0).Select(x => new
        {
            Id =  x.Id,
            Name = x.Username,
            Score = x.LegacyScore,
            Level = 0
        }).OrderByDescending(x => x.Score).ToArrayAsync();

        var leaderboardData = users.Select(x => new
        {
            Username = x.Name,
            Score = x.Score,
            Level = context.UserXps.FirstOrDefault(y => y.Type == XpType.Lifetime && y.User.Id == x.Id)?.CalculateLevel() ?? 0,
        });

        return Ok(new
        {
            Leaderboard = leaderboardData,
        });
    }
}