using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Entities;
using Tetrio.Foxhole.Database.Enums;
using Tetrio.Foxhole.Network.Api.Tetrio;
using Tetrio.Zenith.DailyChallenge.Models;

namespace Tetrio.Zenith.DailyChallenge.Controllers;

public class ZenithUserController(TetrioApi api, TetrioContext context) : BaseController(api)
{
    [HttpGet]
    [Route("{username}/profile")]
    public async Task<ActionResult> GetProfileData(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        var userInfo = await GetTetrioUserInformation(username);

        if (userInfo == null) return NotFound();

        return Ok(userInfo);
    }

    [HttpGet]
    [Route("{username}")]
    public async Task<ActionResult> GetUserData(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();
        username = username.ToLower();
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

        if (user == default) return NotFound();

        var runCount = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).CountAsync();
        var topRun = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).OrderByDescending(x => x.Altitude).FirstOrDefaultAsync();
        var totalGarbageSend = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).SumAsync(x => x.GarbageSent);
        var totalGarbageCleared = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).SumAsync(x => x.GarbageCleared);
        var totalKOs = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).SumAsync(x => x.KOs);
        var totalTimePlayed = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).SumAsync(x => (long) x.TotalTime);

        var today = DateTime.UtcNow;
        var daysSinceMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var startOfWeek = today.Date.AddDays(-daysSinceMonday);

        var topRunWeekly = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.PlayedAt >= startOfWeek).OrderByDescending(x => x.Altitude).FirstOrDefaultAsync();

        var altitudes = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).GroupBy(x => x.User.Id).Select(x => new
        {
            NoMod = Math.Round(x.Where(y => y.Mods.Length == 0).Sum(y => y.Altitude), 2),
            Expert = Math.Round(x.Where(y => y.Mods.Contains("expert")).Sum(y => y.Altitude), 2),
            NoHold = Math.Round(x.Where(y => y.Mods.Contains("nohold")).Sum(y => y.Altitude), 2),
            Messy = Math.Round(x.Where(y => y.Mods.Contains("messy")).Sum(y => y.Altitude), 2),
            Gravity = Math.Round(x.Where(y => y.Mods.Contains("gravity")).Sum(y => y.Altitude), 2),
            Volatile = Math.Round(x.Where(y => y.Mods.Contains("volatile")).Sum(y => y.Altitude), 2),
            DoubleHole = Math.Round(x.Where(y => y.Mods.Contains("doublehole")).Sum(y => y.Altitude), 2),
            Invisible = Math.Round(x.Where(y => y.Mods.Contains("invisible")).Sum(y => y.Altitude), 2),
            AllSpin = Math.Round(x.Where(y => y.Mods.Contains("allspin")).Sum(y => y.Altitude), 2),

            Reverse = Math.Round(x.Where(y => y.Mods.Contains("expert_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("nohold_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("messy_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("gravity_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("volatile_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("doublehole_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("invisible_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("allspin_reversed")).Sum(y => y.Altitude), 2)
        }).FirstOrDefaultAsync();

        var totalAltitude = 0d;
        var percentages = new double[9];

        if(altitudes != null)
        {
            totalAltitude = altitudes.NoMod + altitudes.Expert + altitudes.NoHold + altitudes.Messy + altitudes.Gravity + altitudes.Volatile + altitudes.DoubleHole + altitudes.Invisible + altitudes.AllSpin + altitudes.Reverse;
            percentages =
            [
                Math.Round(altitudes.NoMod / totalAltitude * 100, 2),
                Math.Round(altitudes.Expert / totalAltitude * 100, 2),
                Math.Round(altitudes.NoHold / totalAltitude * 100, 2),
                Math.Round(altitudes.Messy / totalAltitude * 100, 2),
                Math.Round(altitudes.Gravity / totalAltitude * 100, 2),
                Math.Round(altitudes.Volatile / totalAltitude * 100, 2),
                Math.Round(altitudes.DoubleHole / totalAltitude * 100, 2),
                Math.Round(altitudes.Invisible / totalAltitude * 100, 2),
                Math.Round(altitudes.AllSpin / totalAltitude * 100, 2),
                Math.Round(altitudes.Reverse / totalAltitude * 100, 2)
            ];
        }

        var leaderboardDate = DateTime.UtcNow;

        var leaderboard = await context.Leaderboards.AsNoTracking().FirstOrDefaultAsync(x => x.StartDate <= leaderboardDate && (x.EndDate == null || x.EndDate >= leaderboardDate));

        LeaderboardEntry? seasonalScore = null;

        if(leaderboard != null)
            seasonalScore = await context.LeaderboardEntries.FirstOrDefaultAsync(x => x.LeaderboardId == leaderboard.Id && x.User.Id == user.Id);

        return Ok(new
        {
            TetrioId = user.TetrioId,
            Username = user.Username,
            Title = user.Title,
            Score = user.Score,
            SeasonalScore = seasonalScore?.Score ?? 0,
            Runs = runCount,
            TopAltitude = topRun?.Altitude ?? 0,
            TopAltitudeWeekly = topRunWeekly?.Altitude ?? 0,
            GarbageSend = totalGarbageSend,
            GarbageCleared = totalGarbageCleared,
            Kos = totalKOs,
            TimePlayed = (double) totalTimePlayed / 3600000,
            AltitudePercentages = percentages,
        });
    }

    [HttpGet]
    [Route("{username}/extra")]
    public async Task<ActionResult> GetUserDataExtra(string? username, int days = 5)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();
        username = username.ToLower();
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

        if (user == default) return NotFound();

        #region Get Average Values

        var apmAverage = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).AverageAsync(x => x.Apm);
        var apmAverageRecentDays = await context.Runs.AsNoTracking()
            .Where(x => x.User.Id == user.Id && x.PlayedAt != null)
            .GroupBy(x => x.PlayedAt!.Value.Date)
            .OrderByDescending(x => x.Key)
            .Take(days)
            .Select(g => new { Date = g.Key, Average = g.Average(x => x.Apm) })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var vsAverage = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).AverageAsync(x => x.Vs);
        var vsAverageRecentDays = await context.Runs.AsNoTracking()
            .Where(x => x.User.Id == user.Id && x.PlayedAt != null)
            .GroupBy(x => x.PlayedAt!.Value.Date)
            .OrderByDescending(x => x.Key)
            .Take(days)
            .Select(g => new { Date = g.Key, Average = g.Average(x => x.Vs) })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var ppsAverage = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).AverageAsync(x => x.Pps);
        var ppsAverageRecentDays = await context.Runs.AsNoTracking()
            .Where(x => x.User.Id == user.Id && x.PlayedAt != null)
            .GroupBy(x => x.PlayedAt!.Value.Date)
            .OrderByDescending(x => x.Key)
            .Take(days)
            .Select(g => new { Date = g.Key, Average = g.Average(x => x.Pps) })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var altitudeAverage = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).AverageAsync(x => x.Altitude);
        var altitudeAverageRecentDays = await context.Runs.AsNoTracking()
            .Where(x => x.User.Id == user.Id && x.PlayedAt != null)
            .GroupBy(x => x.PlayedAt!.Value.Date)
            .OrderByDescending(x => x.Key)
            .Take(days)
            .Select(g => new { Date = g.Key, Average = g.Average(x => x.Altitude) })
            .OrderBy(x => x.Date)
            .ToListAsync();

        #endregion

        #region Floor Averages

        var floor1Count = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.Altitude >= 0 && x.Altitude < 50 && x.TotalTime > 30000).CountAsync();
        var floor2Count = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.Altitude >= 50 && x.Altitude < 150).CountAsync();
        var floor3Count = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.Altitude >= 150 && x.Altitude < 300).CountAsync();
        var floor4Count = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.Altitude >= 300 && x.Altitude < 450).CountAsync();
        var floor5Count = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.Altitude >= 450 && x.Altitude < 650).CountAsync();
        var floor6Count = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.Altitude >= 650 && x.Altitude < 850).CountAsync();
        var floor7Count = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.Altitude >= 850 && x.Altitude < 1100).CountAsync();
        var floor8Count = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.Altitude >= 1100 && x.Altitude < 1350).CountAsync();
        var floor9Count = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.Altitude >= 1350 && x.Altitude < 1650).CountAsync();
        var floor10Count = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.Altitude >= 1650).CountAsync();

        var floors = new[] { floor1Count, floor2Count, floor3Count, floor4Count, floor5Count, floor6Count, floor7Count, floor8Count, floor9Count, floor10Count };

        #endregion

        return Ok(new
        {
            Floors = new
            {
                Average = floors.Select((count, index) => count * (index + 1d)).Sum() / floors.Sum(),
                Floors = floors
            },
            Apm = new
            {
                Average = apmAverage,
                Recent = apmAverageRecentDays,
                Improvement = apmAverageRecentDays.Count() > 1 ? apmAverageRecentDays[0].Average - apmAverageRecentDays[1].Average : 0
            },
            Vs = new
            {
                Average = vsAverage,
                Recent = vsAverageRecentDays,
                Improvement = vsAverageRecentDays.Count() > 1 ? vsAverageRecentDays[0].Average - vsAverageRecentDays[1].Average : 0
            },
            Pps = new
            {
                Average = ppsAverage,
                Recent = ppsAverageRecentDays,
                Improvement = ppsAverageRecentDays.Count() > 1 ? ppsAverageRecentDays[0].Average - ppsAverageRecentDays[1].Average : 0
            },
            Altitude = new
            {
                Average = altitudeAverage,
                Recent = altitudeAverageRecentDays,
                Improvement = altitudeAverageRecentDays.Count() > 1 ? altitudeAverageRecentDays[0].Average - altitudeAverageRecentDays[1].Average : 0
            }
        });
    }

    [HttpGet]
    [Route("{username}/progression")]
    public async Task<ActionResult> GetUserProgression(string? username, int progressionLimit = 100)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();
        if (progressionLimit < 0) return BadRequest("Progression limit cant be lower than 0");
        if (progressionLimit == 0) progressionLimit = 3000;
        if(progressionLimit > 3000) progressionLimit = 3000;

        progressionLimit = 1000000000;

        username = username.ToLower();

        var user = await context.Users.AsNoTracking().Where(x => x.Username == username).FirstOrDefaultAsync();

        if (user == null) return NotFound();

        var modBaseQuery = context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Type == ProgressionType.Altitude);

        var modProgression = new
        {
            NoMod = await modBaseQuery.Where(x => x.Mods == null).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            Expert = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("expert") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            NoHold = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("nohold") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            Messy = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("messy") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            Gravity = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("gravity") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            Volatile = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("volatile") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            DoubleHole = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("doublehole") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            Invisible = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("invisible") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            AllSpin = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("allspin") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),

            ReverseExpert = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("expert_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseNoHold = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("nohold_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseMessy = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("messy_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseGravity = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("gravity_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseVolatile = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("volatile_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseDoubleHole = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("doublehole_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseInvisible = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("invisible_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseAllspin = await modBaseQuery.Where(x => x.Mods != null && x.Mods.Contains("allspin_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync()
        };

        var splitBaseQuery = context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Type == ProgressionType.ZenithSplit && !x.Mods.Contains("snowman") && !x.Mods.Contains("pento"));

        var splitsProgression = new
        {
            Hotel             = await splitBaseQuery.Where(x => x.Floor == ZenithFloor.Hotel).Select(x => x.Value).OrderByDescending(x => x).Take(progressionLimit).ToArrayAsync(),
            Casino            = await splitBaseQuery.Where(x => x.Floor == ZenithFloor.Casino).Select(x => x.Value).OrderByDescending(x => x).Take(progressionLimit).ToArrayAsync(),
            Arena             = await splitBaseQuery.Where(x => x.Floor == ZenithFloor.Arena).Select(x => x.Value).OrderByDescending(x => x).Take(progressionLimit).ToArrayAsync(),
            Museum            = await splitBaseQuery.Where(x => x.Floor == ZenithFloor.Museum).Select(x => x.Value).OrderByDescending(x => x).Take(progressionLimit).ToArrayAsync(),
            Offices           = await splitBaseQuery.Where(x => x.Floor == ZenithFloor.Offices).Select(x => x.Value).OrderByDescending(x => x).Take(progressionLimit).ToArrayAsync(),
            Laboratory        = await splitBaseQuery.Where(x => x.Floor == ZenithFloor.Laboratory).Select(x => x.Value).OrderByDescending(x => x).Take(progressionLimit).ToArrayAsync(),
            Core              = await splitBaseQuery.Where(x => x.Floor == ZenithFloor.Core).Select(x => x.Value).OrderByDescending(x => x).Take(progressionLimit).ToArrayAsync(),
            Corruption        = await splitBaseQuery.Where(x => x.Floor == ZenithFloor.Corruption).Select(x => x.Value).OrderByDescending(x => x).Take(progressionLimit).ToArrayAsync(),
            PlatformOfTheGods = await splitBaseQuery.Where(x => x.Floor == ZenithFloor.PlatformOfTheGods).Select(x => x.Value).OrderByDescending(x => x).Take(progressionLimit).ToArrayAsync(),
        };

        return Ok(new
        {
            modProgression,
            splitsProgression
        });
    }

    [HttpGet]
    [Route("{username}/daily")]
    public async Task<ActionResult> GetDailyData(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        username = username.ToLower();

        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

        if (user == default) return NotFound();

        var masteryCompletions = await context.MasteryAttempts.AsNoTracking().Where(x => x.UserId == user.Id).GroupBy(x => x.UserId).Select(x => new
        {
            Expert = x.Count(y => y.ExpertCompleted),
            NoHold = x.Count(y => y.NoHoldCompleted),
            Messy = x.Count(y => y.MessyCompleted),
            Gravity = x.Count(y => y.GravityCompleted),
            Volatile = x.Count(y => y.VolatileCompleted),
            DoubleHole = x.Count(y => y.DoubleHoleCompleted),
            Invisible = x.Count(y => y.InvisibleCompleted),
            AllSpin = x.Count(y => y.AllSpinCompleted),

            Reverse =   x.Count(y => y.ExpertReversedCompleted)
                      + x.Count(y => y.NoHoldReversedCompleted)
                      + x.Count(y => y.MessyReversedCompleted)
                      + x.Count(y => y.GravityReversedCompleted)
                      + x.Count(y => y.VolatileReversedCompleted)
                      + x.Count(y => y.DoubleHoleReversedCompleted)
                      + x.Count(y => y.InvisibleReversedCompleted)
                      + x.Count(y => y.AllSpinReversedCompleted)
        }).FirstOrDefaultAsync();

        var altitudes = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).GroupBy(x => x.User.Id).Select(x => new
        {
            NoMod = Math.Round(x.Where(y => y.Mods.Length == 0).Sum(y => y.Altitude), 2),
            Expert = Math.Round(x.Where(y => y.Mods.Contains("expert")).Sum(y => y.Altitude), 2),
            NoHold = Math.Round(x.Where(y => y.Mods.Contains("nohold")).Sum(y => y.Altitude), 2),
            Messy = Math.Round(x.Where(y => y.Mods.Contains("messy")).Sum(y => y.Altitude), 2),
            Gravity = Math.Round(x.Where(y => y.Mods.Contains("gravity")).Sum(y => y.Altitude), 2),
            Volatile = Math.Round(x.Where(y => y.Mods.Contains("volatile")).Sum(y => y.Altitude), 2),
            DoubleHole = Math.Round(x.Where(y => y.Mods.Contains("doublehole")).Sum(y => y.Altitude), 2),
            Invisible = Math.Round(x.Where(y => y.Mods.Contains("invisible")).Sum(y => y.Altitude), 2),
            AllSpin = Math.Round(x.Where(y => y.Mods.Contains("allspin")).Sum(y => y.Altitude), 2),

            Reverse = Math.Round(x.Where(y => y.Mods.Contains("expert_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("nohold_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("messy_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("gravity_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("volatile_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("doublehole_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("invisible_reversed")).Sum(y => y.Altitude)
                            + x.Where(y => y.Mods.Contains("allspin_reversed")).Sum(y => y.Altitude), 2)
        }).FirstOrDefaultAsync();

        var totalAltitude = 0d;
        var percentages = new double[9];

        if(altitudes != null)
        {
            totalAltitude = altitudes.NoMod + altitudes.Expert + altitudes.NoHold + altitudes.Messy + altitudes.Gravity + altitudes.Volatile + altitudes.DoubleHole + altitudes.Invisible + altitudes.AllSpin + altitudes.Reverse;
            percentages =
            [
                Math.Round(altitudes.NoMod / totalAltitude * 100, 2),
                Math.Round(altitudes.Expert / totalAltitude * 100, 2),
                Math.Round(altitudes.NoHold / totalAltitude * 100, 2),
                Math.Round(altitudes.Messy / totalAltitude * 100, 2),
                Math.Round(altitudes.Gravity / totalAltitude * 100, 2),
                Math.Round(altitudes.Volatile / totalAltitude * 100, 2),
                Math.Round(altitudes.DoubleHole / totalAltitude * 100, 2),
                Math.Round(altitudes.Invisible / totalAltitude * 100, 2),
                Math.Round(altitudes.AllSpin / totalAltitude * 100, 2),
                Math.Round(altitudes.Reverse / totalAltitude * 100, 2)
            ];
        }

        var runCount = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).CountAsync();
        var splitsCount = await context.ZenithSplits.AsNoTracking().Where(x => x.User.Id == user.Id).CountAsync();
        var daysParticipated = await context.Users.AsNoTracking().Where(x => x.Username == username).SelectMany(x => x.Challenges).OrderByDescending(x => x.Date).Select(x => x.Date).GroupBy(x => x).CountAsync();

        var totalChallengesCompleted = await context.Users.AsNoTracking().Where(x => x.Username == username).SelectMany(x => x.Challenges).CountAsync();
        var challengesCompleted = await context.Users.AsNoTracking().Where(x => x.Username == username).SelectMany(x => x.Challenges).GroupBy(x => x.Date).CountAsync();

        var userXp = await context.UserXps.Where(x => x.User.Id == user.Id).ToArrayAsync();

        var lifetimeXp = userXp.FirstOrDefault(x => x.Type == XpType.Lifetime);

        var leaderboardDate = DateTime.UtcNow;

        var leaderboard = await context.Leaderboards.AsNoTracking().FirstOrDefaultAsync(x => x.StartDate <= leaderboardDate && (x.EndDate == null || x.EndDate >= leaderboardDate));

        LeaderboardEntry? seasonalScore = null;

        if(leaderboard != null)
            seasonalScore = await context.LeaderboardEntries.FirstOrDefaultAsync(x => x.LeaderboardId == leaderboard.Id && x.User.Id == user.Id);

        if (masteryCompletions != null)
        {
            totalChallengesCompleted += masteryCompletions.Expert;
            totalChallengesCompleted += masteryCompletions.NoHold;
            totalChallengesCompleted += masteryCompletions.Messy;
            totalChallengesCompleted += masteryCompletions.Gravity;
            totalChallengesCompleted += masteryCompletions.Volatile;
            totalChallengesCompleted += masteryCompletions.DoubleHole;
            totalChallengesCompleted += masteryCompletions.Invisible;
            totalChallengesCompleted += masteryCompletions.AllSpin;

            totalChallengesCompleted += masteryCompletions.Reverse;
        }

        return Ok(new
        {
            UserInfo = new
            {
                UserId = user.TetrioId,
                user.Username,
                TetrioRank = user.TetrioRank ?? "z",
                TotalXP = lifetimeXp?.TotalXp ?? 0,
                Level = lifetimeXp?.CalculateLevel() ?? 1
            },
            user.TetrioId,
            Runs = runCount,
            Splits = splitsCount,
            ChallengesCompleted = challengesCompleted,
            TotalChallengesCompleted = totalChallengesCompleted,
            DaysParticipated = daysParticipated,
            Altitudes = altitudes,
            AltitudePercentages = percentages,
            MasteryCompletions = masteryCompletions,
            Score = user.Score,
            SeasonalScore = seasonalScore?.Score ?? 0,
        });
    }

    [HttpGet]
    [Route("{username}/dailyExtra")]
    public async Task<ActionResult> GetDailyExtra(string username, int progressionLimit = 100)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();
        if (progressionLimit < 0) return BadRequest("Progression limit cant be lower than 0");
        if (progressionLimit == 0) progressionLimit = 1000;
        if(progressionLimit > 1000) progressionLimit = 1000;

        progressionLimit = 1000000000;

        username = username.ToLower();

        var user = await context.Users.AsNoTracking().Where(x => x.Username == username).FirstOrDefaultAsync();

        if (user == null) return NotFound();

        var sevenDaysAgo = DateTime.UtcNow.AddDays(-14);

        var recentDays = await context.Runs.AsNoTracking()
            .Where(x => x.UserId == user.Id && x.PlayedAt >= sevenDaysAgo)
            .GroupBy(x => x.PlayedAt!.Value.Date)
            .Select(x => new
            {
                Date = x.Key.ToString("d. MMM"),
                Altitude = new
                {
                    Max = x.Max(y => y.Altitude),
                    Avg = x.Average(y => y.Altitude)
                },
                APM = new
                {
                    Max = x.Max(y => y.Apm),
                    Avg = x.Average(y => y.Apm)
                },
                VS = new
                {
                    Max = x.Max(y => y.Vs),
                    Avg = x.Average(y => y.Vs)
                }
            }).ToArrayAsync();

        var modProgression = new
        {
            NoMod = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods == null).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            Expert = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("expert") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            NoHold = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("nohold") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            Messy = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("messy") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            Gravity = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("gravity") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            Volatile = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("volatile") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            DoubleHole = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("doublehole") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            Invisible = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("invisible") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            AllSpin = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("allspin") && !x.Mods.Contains("_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),

            ReverseExpert = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("expert_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseNoHold = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("nohold_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseMessy = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("messy_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseGravity = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("gravity_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseVolatile = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("volatile_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseDoubleHole = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("doublehole_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseInvisible = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("invisible_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync(),
            ReverseAllspin = await context.Progressions.AsNoTracking().Where(x => x.UserId == user.Id && x.Mods != null && x.Mods.Contains("allspin_reversed")).Select(x => Math.Round(x.Value, 2)).OrderBy(x => x).Take(progressionLimit).ToArrayAsync()
        };

        return Ok(new
        {
            RecentDays = recentDays,
            ModProgression = modProgression
        });
    }

    [HttpGet]
    [Route("{username}/run/{runId}")]
    public async Task<ActionResult> GetRun(string username, string runId)
    {
        context.ChangeTracker.LazyLoadingEnabled = false;

        var run = await context.Runs.AsNoTracking().Where(x => x.TetrioId == runId && x.User.Username == username).FirstOrDefaultAsync();
        var split = await context.ZenithSplits.AsNoTracking().Where(x => x.TetrioId == runId).FirstOrDefaultAsync();

        return Ok(new
        {
            Run = run,
            Split = split
        });
    }

    [HttpGet]
    [Route("{username}/runs")]
    public async Task<ActionResult> GetRuns(string username, int page = 0, int pageSize = 25)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        var runs = await context.Runs
            .AsNoTracking()
            .Where(x => x.User.Username == username)
            .OrderByDescending(x => x.PlayedAt)
            .Skip(page * pageSize).Take(pageSize)
            .Select(x => new
            {
                x.PlayedAt,
                x.TetrioId,
                x.Mods,
                x.Altitude,
                x.Quads,
                x.Spins,
                x.AllClears,
                ko = x.KOs,
                x.Apm,
                x.Pps,
                x.Vs,
                x.Finesse,
                x.Back2Back,
                x.SpeedrunSeen,
                x.SpeedrunCompleted
            }).ToArrayAsync();

        return Ok(runs);
    }

    [HttpGet]
    [Route("{username}/bestSplits")]
    public async Task<ActionResult> GetBestSplits(string username, string? mod = null, bool soloMod = false)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        mod ??= string.Empty;

        username = username.ToLower();

        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

        if (user == default) return NotFound();

        var splitsQuery = context.ZenithSplits.AsNoTracking().Where(x => x.User.Id == user.Id);

        if (!string.IsNullOrWhiteSpace(mod))
        {
            if (mod == "nomod")
            {
                splitsQuery = splitsQuery.Where(x => x.Mods == null || x.Mods.Length == 0);
            }
            else
            {
                // Remove event mods from splits
                splitsQuery = splitsQuery.Where(x => x.Mods != null && x.Mods.Length > 0 && x.Mods.Contains(mod) && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento")));

                if (soloMod) splitsQuery = splitsQuery.Where(x => x.Mods == mod);
            }
        }
        else
        {
            // Remove event mods from splits
            splitsQuery = splitsQuery.Where(x => x.Mods != null && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento")));
        }

        var splitData = await splitsQuery
            .GroupBy(x => x.User.Id)
            .Select(group => new
            {
                Mods = mod,
                SplitAverages = new
                {
                    Hotel = group.Where(x => x.HotelReachedAt > 0).OrderByDescending(x => x.DatePlayed).Take(500).Average(x => (double?)x.HotelReachedAt) ?? 0,
                    Casino = group.Where(x => x.CasinoReachedAt > 0).OrderByDescending(x => x.DatePlayed).Take(500).Average(x => (double?)x.CasinoReachedAt) ?? 0,
                    Arena = group.Where(x => x.ArenaReachedAt > 0).OrderByDescending(x => x.DatePlayed).Take(500).Average(x => (double?)x.ArenaReachedAt) ?? 0,
                    Museum = group.Where(x => x.MuseumReachedAt > 0).OrderByDescending(x => x.DatePlayed).Take(500).Average(x => (double?)x.MuseumReachedAt) ?? 0,
                    Offices = group.Where(x => x.OfficesReachedAt > 0).OrderByDescending(x => x.DatePlayed).Take(500).Average(x => (double?)x.OfficesReachedAt) ?? 0,
                    Laboratory = group.Where(x => x.LaboratoryReachedAt > 0).OrderByDescending(x => x.DatePlayed).Take(500).Average(x => (double?)x.LaboratoryReachedAt) ?? 0,
                    Core = group.Where(x => x.CoreReachedAt > 0).OrderByDescending(x => x.DatePlayed).Take(500).Average(x => (double?)x.CoreReachedAt) ?? 0,
                    Corruption = group.Where(x => x.CorruptionReachedAt > 0).OrderByDescending(x => x.DatePlayed).Take(500).Average(x => (double?)x.CorruptionReachedAt) ?? 0,
                    PlatformOfTheGods = group.Where(x => x.PlatformOfTheGodsReachedAt > 0).OrderByDescending(x => x.DatePlayed).Take(500).Average(x => (double?)x.PlatformOfTheGodsReachedAt) ?? 0
                },
                GoldSplits = new
                {
                    Hotel = group.Where(x => x.HotelReachedAt > 0).Min(x => (int?)x.HotelReachedAt) ?? 0,
                    Casino = group.Where(x => x.CasinoReachedAt > 0).Min(x => (int?)x.CasinoReachedAt) ?? 0,
                    Arena = group.Where(x => x.ArenaReachedAt > 0).Min(x => (int?)x.ArenaReachedAt) ?? 0,
                    Museum = group.Where(x => x.MuseumReachedAt > 0).Min(x => (int?)x.MuseumReachedAt) ?? 0,
                    Offices = group.Where(x => x.OfficesReachedAt > 0).Min(x => (int?)x.OfficesReachedAt) ?? 0,
                    Laboratory = group.Where(x => x.LaboratoryReachedAt > 0).Min(x => (int?)x.LaboratoryReachedAt) ?? 0,
                    Core = group.Where(x => x.CoreReachedAt > 0).Min(x => (int?)x.CoreReachedAt) ?? 0,
                    Corruption = group.Where(x => x.CorruptionReachedAt > 0).Min(x => (int?)x.CorruptionReachedAt) ?? 0,
                    PlatformOfTheGods = group.Where(x => x.PlatformOfTheGodsReachedAt > 0).Min(x => (int?)x.PlatformOfTheGodsReachedAt) ?? 0
                },
                GoldAchievedDate = new
                {
                    Hotel = group.Where(x => x.HotelReachedAt > 0).OrderBy(x => x.HotelReachedAt).Select(x => x.DatePlayed).FirstOrDefault(),
                    Casino = group.Where(x => x.CasinoReachedAt > 0).OrderBy(x => x.CasinoReachedAt).Select(x => x.DatePlayed).FirstOrDefault(),
                    Arena = group.Where(x => x.ArenaReachedAt > 0).OrderBy(x => x.ArenaReachedAt).Select(x => x.DatePlayed).FirstOrDefault(),
                    Museum = group.Where(x => x.MuseumReachedAt > 0).OrderBy(x => x.MuseumReachedAt).Select(x => x.DatePlayed).FirstOrDefault(),
                    Offices = group.Where(x => x.OfficesReachedAt > 0).OrderBy(x => x.OfficesReachedAt).Select(x => x.DatePlayed).FirstOrDefault(),
                    Laboratory = group.Where(x => x.LaboratoryReachedAt > 0).OrderBy(x => x.LaboratoryReachedAt).Select(x => x.DatePlayed).FirstOrDefault(),
                    Core = group.Where(x => x.CoreReachedAt > 0).OrderBy(x => x.CoreReachedAt).Select(x => x.DatePlayed).FirstOrDefault(),
                    Corruption = group.Where(x => x.CorruptionReachedAt > 0).OrderBy(x => x.CorruptionReachedAt).Select(x => x.DatePlayed).FirstOrDefault(),
                    PlatformOfTheGods = group.Where(x => x.PlatformOfTheGodsReachedAt > 0).OrderBy(x => x.PlatformOfTheGodsReachedAt).Select(x => x.DatePlayed).FirstOrDefault()
                }
            }).Select(x => new
            {
                Hotel = new ZenithSplitResult(x.Mods, x.GoldAchievedDate.Hotel, (uint)x.GoldSplits.Hotel, x.SplitAverages.Hotel),
                Casino = new ZenithSplitResult(x.Mods, x.GoldAchievedDate.Casino, (uint)x.GoldSplits.Casino, x.SplitAverages.Casino),
                Arena = new ZenithSplitResult(x.Mods, x.GoldAchievedDate.Arena, (uint)x.GoldSplits.Arena, x.SplitAverages.Arena),
                Museum = new ZenithSplitResult(x.Mods, x.GoldAchievedDate.Museum, (uint)x.GoldSplits.Museum, x.SplitAverages.Museum),
                Offices = new ZenithSplitResult(x.Mods, x.GoldAchievedDate.Offices, (uint)x.GoldSplits.Offices, x.SplitAverages.Offices),
                Laboratory = new ZenithSplitResult(x.Mods, x.GoldAchievedDate.Laboratory, (uint)x.GoldSplits.Laboratory, x.SplitAverages.Laboratory),
                Core = new ZenithSplitResult(x.Mods, x.GoldAchievedDate.Core, (uint)x.GoldSplits.Core, x.SplitAverages.Core),
                Corruption = new ZenithSplitResult(x.Mods, x.GoldAchievedDate.Corruption, (uint)x.GoldSplits.Corruption, x.SplitAverages.Corruption),
                PlatformOfTheGods = new ZenithSplitResult(x.Mods, x.GoldAchievedDate.PlatformOfTheGods, (uint)x.GoldSplits.PlatformOfTheGods, x.SplitAverages.PlatformOfTheGods)
            }).SingleOrDefaultAsync();

        return Ok(new
        {
            Hotel = new
            {
                AverageTime = splitData?.Hotel.ToAverageTimeString(),
                BestTime = splitData?.Hotel.ToGoldTimeString(),
                BestTimeAchievedDate = splitData?.Hotel.ToDateAchievedString()
            },
            Casino = new
            {
                AverageTime = splitData?.Casino.ToAverageTimeString(),
                BestTime = splitData?.Casino.ToGoldTimeString(),
                BestTimeAchievedDate = splitData?.Casino.ToDateAchievedString()
            },
            Arena = new
            {
                AverageTime = splitData?.Arena.ToAverageTimeString(),
                BestTime = splitData?.Arena.ToGoldTimeString(),
                BestTimeAchievedDate = splitData?.Arena.ToDateAchievedString()
            },
            Museum = new
            {
                AverageTime = splitData?.Museum.ToAverageTimeString(),
                BestTime = splitData?.Museum.ToGoldTimeString(),
                BestTimeAchievedDate = splitData?.Museum.ToDateAchievedString()
            },
            Offices = new
            {
                AverageTime = splitData?.Offices.ToAverageTimeString(),
                BestTime = splitData?.Offices.ToGoldTimeString(),
                BestTimeAchievedDate = splitData?.Offices.ToDateAchievedString()
            },
            Laboratory = new
            {
                AverageTime = splitData?.Laboratory.ToAverageTimeString(),
                BestTime = splitData?.Laboratory.ToGoldTimeString(),
                BestTimeAchievedDate = splitData?.Laboratory.ToDateAchievedString()
            },
            Core = new
            {
                AverageTime = splitData?.Core.ToAverageTimeString(),
                BestTime = splitData?.Core.ToGoldTimeString(),
                BestTimeAchievedDate = splitData?.Core.ToDateAchievedString()
            },
            Corruption = new
            {
                AverageTime = splitData?.Corruption.ToAverageTimeString(),
                BestTime = splitData?.Corruption.ToGoldTimeString(),
                BestTimeAchievedDate = splitData?.Corruption.ToDateAchievedString()
            },
            Potg = new
            {
                AverageTime = splitData?.PlatformOfTheGods.ToAverageTimeString(),
                BestTime = splitData?.PlatformOfTheGods.ToGoldTimeString(),
                BestTimeAchievedDate = splitData?.PlatformOfTheGods.ToDateAchievedString()
            }
        });
    }

    [HttpGet]
    [Route("{username}/splits")]
    public async Task<ActionResult> GetSplits(string? username, int page = 0, int pageSize = 25)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        var splits = await context.ZenithSplits
            .AsNoTracking()
            .Where(x => x.User.Username == username)
            .OrderByDescending(x => x.CreatedAt)
            .Skip(page * pageSize).Take(pageSize)
            .Select(x => new
            {
                x.TetrioId,
                Hotel = x.HotelReachedAt,
                Casino = x.CasinoReachedAt,
                Arena = x.ArenaReachedAt,
                Museum = x.MuseumReachedAt,
                Offices = x.OfficesReachedAt,
                Laboratory = x.LaboratoryReachedAt,
                Core = x.CoreReachedAt,
                Corruption = x.CorruptionReachedAt,
                Potg = x.PlatformOfTheGodsReachedAt
            })
            .ToArrayAsync();

        return Ok(splits.Select(x => new
        {
            x.TetrioId,
            Hotel = x.Hotel > 0 ? TimeSpan.FromMilliseconds(x.Hotel).ToString(@"mm\:ss\.fff") : "-",
            Casino = x.Casino > 0 ? TimeSpan.FromMilliseconds(x.Casino).ToString(@"mm\:ss\.fff") : "-",
            Arena = x.Arena > 0 ? TimeSpan.FromMilliseconds(x.Arena).ToString(@"mm\:ss\.fff") : "-",
            Museum = x.Museum > 0 ? TimeSpan.FromMilliseconds(x.Museum).ToString(@"mm\:ss\.fff") : "-",
            Offices = x.Offices > 0 ? TimeSpan.FromMilliseconds(x.Offices).ToString(@"mm\:ss\.fff") : "-",
            Laboratory = x.Laboratory > 0 ? TimeSpan.FromMilliseconds(x.Laboratory).ToString(@"mm\:ss\.fff") : "-",
            Core = x.Core > 0 ? TimeSpan.FromMilliseconds(x.Core).ToString(@"mm\:ss\.fff") : "-",
            Corruption = x.Corruption > 0 ? TimeSpan.FromMilliseconds(x.Corruption).ToString(@"mm\:ss\.fff") : "-",
            Potg = x.Potg > 0 ? TimeSpan.FromMilliseconds(x.Potg).ToString(@"mm\:ss\.fff") : "-"
        }));
    }

    [HttpGet]
    [Route("{username}/challenges")]
    public async Task<ActionResult> GetChallenges(string? username, int page = 0, int pageSize = 25)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        var runs = await context.Users
            .AsNoTracking()
            .Where(x => x.Username == username)
            .SelectMany(x => x.Challenges)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Points)
            .Skip(page * pageSize).Take(pageSize)
            .Select(x => new
            {
                x.Date,
                Difficulty = x.Points,
                x.Mods,
                Conditions = x.Conditions.Select(a => new
                {
                    a.ChallengeId,
                    a.Type,
                    a.Value
                })
            }).ToArrayAsync();

        return Ok(runs);
    }

    [HttpGet]
    [Route("{username}/challengeCompletions")]
    public async Task<ActionResult> GetChallengeCompletions(string? username, int page = 0, int pageSize = 25)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        var runs = (await context.Users
                .AsNoTracking()
                .Where(x => x.Username == username)
                .SelectMany(x => x.Challenges)
                .Select(x => new
                {
                    x.Date,
                    Difficulty = x.Points,
                    x.Mods,
                    Conditions = x.Conditions.Select(a => new
                    {
                        a.ChallengeId,
                        a.Type,
                        a.Value
                    })
                })
                .GroupBy(x => x.Date)
                .ToArrayAsync())
            .OrderByDescending(x => x.Key)
            .Skip(page * pageSize).Take(pageSize);

        context.ChangeTracker.LazyLoadingEnabled = true;

        var woms = await context.MasteryAttempts.AsNoTracking().Where(x => x.User.Username == username && runs.Select(y => y.Key).Contains(x.MasteryChallenge.Date))
            .Select(x => new
            {
                Date = x.MasteryChallenge.Date,

                x.ExpertCompleted,
                x.NoHoldCompleted,
                x.MessyCompleted,
                x.GravityCompleted,
                x.VolatileCompleted,
                x.DoubleHoleCompleted,
                x.InvisibleCompleted,
                x.AllSpinCompleted,

                x.ExpertReversedCompleted,
                x.NoHoldReversedCompleted,
                x.MessyReversedCompleted,
                x.GravityReversedCompleted,
                x.VolatileReversedCompleted,
                x.DoubleHoleReversedCompleted,
                x.InvisibleReversedCompleted,
                x.AllSpinReversedCompleted,
            }).ToArrayAsync();

        var data = runs.Select(x =>
        {
            var date = x.Key;

            var wom = woms.FirstOrDefault(y => y.Date == date);

            var veryEasyCompleted = false;
            var easyCompleted = false;
            var normalCompleted = false;
            var hardCompleted = false;
            var expertCompleted = false;
            var reverseCompleted = false;

            foreach (var challenge in x)
            {
                switch ((Difficulty)challenge.Difficulty)
                {
                    case Difficulty.VeryEasy:
                        veryEasyCompleted = true;
                        break;
                    case Difficulty.Easy:
                        easyCompleted = true;
                        break;
                    case Difficulty.Normal:
                        normalCompleted = true;
                        break;
                    case Difficulty.Hard:
                        hardCompleted = true;
                        break;
                    case Difficulty.Expert:
                        expertCompleted = true;
                        break;
                    case Difficulty.Reverse:
                        reverseCompleted = true;
                        break;
                }
            }

            return new
            {
                Date = date,
                VeryEasyCompleted = veryEasyCompleted,
                EasyCompleted = easyCompleted,
                NormalCompleted = normalCompleted,
                HardCompleted = hardCompleted,
                ExpertCompleted = expertCompleted,
                ReverseCompleted = reverseCompleted,
                Mastery = wom
            };
        }).OrderByDescending(x => x.Date).ToArray();

        return Ok(data);
    }

    [HttpGet]
    [Route("{username}/getTodaysChallengeCompletions")]
    public async Task<ActionResult> GetTodaysChallengeCompletions(string username)
    {
        username = username.ToLower();

        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

        if (user == null) return Ok($"User '{username}' not found");

        var utc = DateTime.UtcNow;

        var date = new DateOnly(utc.Year, utc.Month, utc.Day);

        var challenges = await context.Users
            .AsNoTracking()
            .Where(x => x.Username == username)
            .SelectMany(x => x.Challenges)
            .Where(x => x.Date == date)
            .Select(x => new
            {
                x.Id,
                x.Date,
                Difficulty = x.Points,
                x.Mods,
                Conditions = x.Conditions.Select(a => new
                {
                    a.ChallengeId,
                    a.Type,
                    a.Value
                })
            }).ToArrayAsync();

        var veryEasyCompleted = false;
        var easyCompleted = false;
        var normalCompleted = false;
        var hardCompleted = false;
        var expertCompleted = false;
        var reverseCompleted = false;

        var ids = challenges.Select(x => x.Id).ToArray();

        foreach (var challenge in challenges)
            switch ((Difficulty)challenge.Difficulty)
            {
                case Difficulty.VeryEasy:
                    veryEasyCompleted = true;
                    break;
                case Difficulty.Easy:
                    easyCompleted = true;
                    break;
                case Difficulty.Normal:
                    normalCompleted = true;
                    break;
                case Difficulty.Hard:
                    hardCompleted = true;
                    break;
                case Difficulty.Expert:
                    expertCompleted = true;
                    break;
                case Difficulty.Reverse:
                    reverseCompleted = true;
                    break;
            }

        var masteryChallenge = await context.Users
            .AsNoTracking()
            .Where(x => x.Username == username)
            .SelectMany(x => x.MasteryAttempts)
            .Where(x => x.MasteryChallenge != null && x.MasteryChallenge.Date == date)
            .Select(x => new
            {
                x.ExpertCompleted,
                x.NoHoldCompleted,
                x.MessyCompleted,
                x.GravityCompleted,
                x.VolatileCompleted,
                x.DoubleHoleCompleted,
                x.InvisibleCompleted,
                x.AllSpinCompleted,

                x.ExpertReversedCompleted,
                x.NoHoldReversedCompleted,
                x.MessyReversedCompleted,
                x.GravityReversedCompleted,
                x.VolatileReversedCompleted,
                x.DoubleHoleReversedCompleted,
                x.InvisibleReversedCompleted,
                x.AllSpinReversedCompleted,
            }).FirstOrDefaultAsync();

        return Ok(new
        {
            CompletedChallengesIds = ids,
            Date = date,
            VeryEasyCompleted = veryEasyCompleted,
            EasyCompleted = easyCompleted,
            NormalCompleted = normalCompleted,
            HardCompleted = hardCompleted,
            ExpertCompleted = expertCompleted,
            ReverseCompleted = reverseCompleted,
            MasteryChallenge = masteryChallenge
        });
    }

    [HttpGet]
    [Route("{username}/getCommunityContributions")]
    public async Task<ActionResult> GetCommunityContributions(string? username, int page = 0, int pageSize = 25)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        var user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);

        if (user == null) return NotFound($"User '{username}' not found");

        var challenges = await context.CommunityChallenges
            .AsNoTracking()
            .Where(x => x.Contributions.Any(y => y.UserId == user.Id && !y.IsLate))
            .OrderByDescending(x => x.StartDate)
            .Skip(page * pageSize).Take(pageSize)
            .Select(x => new
            {
                CommunityChallengeId = x.Id,
                Date = x.StartDate,
                Challenge = string.IsNullOrWhiteSpace(x.Name) ? $"{x.StartDate:yyyy-MM-dd}" : x.Name,
                x.TargetValue,
                x.ConditionType
            })
            .ToArrayAsync();

        var contributionsCount = await context.CommunityChallenges
            .AsNoTracking()
            .Where(x => x.Contributions.Any(y => y.UserId == user.Id && !y.IsLate))
            .CountAsync();

        var challengeIds = challenges.Select(x => x.CommunityChallengeId).ToArray();

        var userContributions = await context.CommunityContributions
            .AsNoTracking()
            .Where(x => challengeIds.Contains(x.CommunityChallengeId) && x.UserId == user.Id && !x.IsLate)
            .GroupBy(x => x.CommunityChallengeId)
            .Select(g => new
            {
                CommunityChallengeId = g.Key,
                TotalAmountContributed = Math.Round(g.Sum(x => x.Amount), 2)
            })
            .ToArrayAsync();

        var participantStats = await context.CommunityContributions
            .AsNoTracking()
            .Where(x => challengeIds.Contains(x.CommunityChallengeId) && !x.IsLate)
            .GroupBy(x => new { x.CommunityChallengeId, x.UserId })
            .Select(g => new
            {
                g.Key.CommunityChallengeId,
                g.Key.UserId,
                TotalAmountContributed = g.Sum(x => x.Amount)
            })
            .ToArrayAsync();

        var result = challenges.Select(x =>
        {
            var participants = participantStats.Where(p => p.CommunityChallengeId == x.CommunityChallengeId).ToArray();

            var placement = participants
                .OrderByDescending(p => p.TotalAmountContributed)
                .ThenBy(p => p.UserId)
                .Select((p, index) => new { p.UserId, Placement = index + 1 })
                .FirstOrDefault(p => p.UserId == user.Id)?.Placement ?? 0;

            var totalAmountContributed = userContributions.FirstOrDefault(y => y.CommunityChallengeId == x.CommunityChallengeId)?.TotalAmountContributed ?? 0;

            return new
            {
                x.Date,
                x.Challenge,
                TotalAmountContributed = totalAmountContributed,
                ContributionPercentage = totalAmountContributed / x.TargetValue * 100,
                x.ConditionType,
                Placement = placement,
                ParticipantCount = participants.Length,
                TotalContributions = contributionsCount
            };
        });

        return Ok(result);
    }

    [HttpGet]
    [Route("search")]
    public async Task<ActionResult> SearchUser(string? query)
    {
        if (string.IsNullOrWhiteSpace(query)) return BadRequest("Query parameter is required");

        query = query.ToLower();

        var foundUsers = await context.Users.AsNoTracking().Where(x => x.Username.Contains(query)).ToArrayAsync();

        return Ok(foundUsers);
    }

    [HttpGet]
    [Route("{username}/seasonalHistory")]
    public async Task<ActionResult> GetSeasonalHistory(string username)
    {
        username = username.ToLower();

        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

        if(user == null) return NotFound($"User '{username}' not found");

        var leaderboards = await context.Leaderboards.AsNoTracking().OrderByDescending(x => x.StartDate).Select(x => new
        {
            x.Id,
            x.Name,
        }).ToArrayAsync();

        List<dynamic> result = new();

        foreach (var leaderboard in leaderboards)
        {
            var entry = await context.LeaderboardEntries.AsNoTracking().FirstOrDefaultAsync(x => x.LeaderboardId == leaderboard.Id && x.UserId == user.Id);

            if(entry == null) continue;

            var participants = await context.LeaderboardEntries.CountAsync(x => x.LeaderboardId == leaderboard.Id);
            // calculate position of user in leaderboard
            var position = await context.LeaderboardEntries.AsNoTracking().CountAsync(x => x.LeaderboardId == leaderboard.Id && x.Score > entry.Score) + 1;

            result.Add(new
            {
                SeasonName = leaderboard.Name,
                SeasonPlacement = position,
                SeasonParticipants = participants,
                SeasonScore = entry.Score
            });
        }

        return Ok(result);
    }

    #if DEBUG
    [HttpGet]
    [Route("convertLegacyScore")]
    public async Task<ActionResult> ConvertLegacyScore()
    {
        var legacyUsersWithScore = await context.Users.AsNoTracking().Select(x => new
        {
            UserId = x.Id,
            Username = x.Username,
            NormalScore = x.Challenges
                .Where(y => y.Points != (byte)Difficulty.Expert && y.Points != (byte)Difficulty.Reverse)
                .Sum(y => y.Points),
            ExpertScore = x.Challenges.Where(y => y.Points == (byte)Difficulty.Expert).Sum(y => y.Points),
            ReverseScore = x.Challenges.Where(y => y.Points == (byte)Difficulty.Reverse).Sum(y => y.Points),
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
        }).ToArrayAsync();

        var userScores = legacyUsersWithScore.Select(x => new
        {
            UserId = x.UserId,
            Username = x.Username,
            Score = x == null
                ? 0
                : Math.Round(x.NormalScore + x.ExpertScore + x.MasteryScore * 2 + x.ReverseScore / 2d, 0)
        }).OrderByDescending(x => x.Score);

        int rowsUpdated = 0;

        foreach (var userData in userScores.Where(x => x.Score > 0))
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userData.UserId);

            if(user == null) continue;

            user.Score = (uint) userData.Score;
            user.LegacyScore = user.Score;

            rowsUpdated += await context.SaveChangesAsync();
        }

        return Ok(rowsUpdated);
    }

    [HttpGet]
    [Route("convertNewLegacyScore")]
    public async Task<ActionResult> ConvertNewLegacyScore()
    {
        var usersWithScore = await context.Users.AsNoTracking().Where(x => x.Score > 0).Select(x => new
                {
                    UserId = x.Id,
                    Username = x.Username,
                    Score = x.Score
                }).OrderByDescending(x => x.Score).ToArrayAsync();

        int rowsUpdated = 0;

        foreach (var userData in usersWithScore)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userData.UserId);

            if(user == null) continue;

            user.LegacyScore = user.Score;

            rowsUpdated += await context.SaveChangesAsync();
        }

        return Ok(rowsUpdated);
    }

    [HttpGet]
    [Route("calculateXpForAllUsersWithoutXP")]
    public async Task<IActionResult> CalculateXpForAllUsersWithoutXP()
    {
        var userIds = await context.Users.AsNoTracking().Where(x => x.Xp.Count == 0 && x.Runs.Count > 0).Select(x => x.Id).ToArrayAsync();

        Console.WriteLine($"[XP CALC] Users without XP: {userIds.Length}");

        var entriesSaved = 0;

        var totalSw = new Stopwatch();
        totalSw.Restart();

        foreach (var userId in userIds)
        {
            var sw = new Stopwatch();
            sw.Restart();

            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var runs = await context.Runs.AsNoTracking().Where(x => x.UserId == userId).ToArrayAsync();

            if (user == null) continue;
            if (runs.Length == 0) continue;

            Console.WriteLine($"[XP CALC] [#{userIds.IndexOf(userId) + 1}/{userIds.Length}] Calculating XP for user {user.Username}");

            var normalRuns = runs.Where(x => !x.Mods.Contains("expert")).ToArray();
            var expertRuns = runs.Where(x => x.Mods.Contains("expert")).ToArray();

            var totalAltitudeXp = (long)normalRuns.Sum(x => x.Altitude) * 1;
            var totalExpertAltitudeXp = (long)(expertRuns.Sum(x => x.Altitude) * 1.5);
            var totalTimeXp = (long)(TimeSpan.FromMilliseconds(normalRuns.Sum(x => x.TotalTime)).TotalMinutes * 100);
            var totalExpertTimeXp = (long)(TimeSpan.FromMilliseconds(expertRuns.Sum(x => x.TotalTime)).TotalMinutes * 150);

            var totalXp = (totalAltitudeXp + totalTimeXp + totalExpertAltitudeXp + totalExpertTimeXp);

            var xp = new UserXp
            {
                TotalXp = totalXp,
                User = user,
                Type = XpType.Lifetime,
            };

            sw.Stop();
            Console.WriteLine($"[XP CALC] Calculated XP: {totalXp} -> Level: {UserXp.CalculateLevelFromTotalXp(totalXp)} | Took: {sw.ElapsedMilliseconds}ms");

            await context.UserXps.AddAsync(xp);
            var saveAmount = await context.SaveChangesAsync();


            entriesSaved += saveAmount;
        }

        Console.WriteLine($"[XP CALC] Saved {entriesSaved} entries in database. Took {totalSw.Elapsed:g}");

        return Ok(entriesSaved);
    }

    [HttpGet]
    [Route("migrateProgressions")]
    public async Task<IActionResult> MigrateProgressions()
    {
        var users = await context.Users.AsNoTracking().ToListAsync();

        var totalProgressions = 0;

        foreach (var user in users)
        {
            var runs =  await context.Runs.AsNoTracking().Where(x => x.UserId == user.Id).OrderBy(x => x.PlayedAt).ToListAsync();

            if (runs == null || runs.Count == 0)
            {
                Console.WriteLine($"[PROGRESSIONS] User {user.Username} has no runs");
                continue;
            }

            var splits = await context.ZenithSplits.AsNoTracking().Where(x => x.User.Id == user.Id).ToListAsync();

            var progressions = new List<Progression>();

            var sw = new Stopwatch();
            sw.Start();

            foreach (var run in runs)
            {
                var isPb = !progressions.Any(x => x.Type == ProgressionType.Altitude && x.Value > run.Altitude && string.IsNullOrWhiteSpace(x.Mods));

                if (string.IsNullOrWhiteSpace(run.Mods))
                {
                    var progression = new Progression()
                    {
                        UserId = user.Id,
                        TetrioId = run.TetrioId,
                        Value = run.Altitude,
                        Type = ProgressionType.Altitude,
                        Mods = null,
                        PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                        IsPersonalBest = isPb
                    };

                    if (progression.IsPersonalBest)
                    {
                        progressions.Add(progression);
                        context.Add(progression);
                    }
                }
                else
                {
                    var mods = run.Mods.Split(' ');

                    foreach (var mod in mods)
                    {
                        if(mod.Contains("snowman") || mod.Contains("pento")) continue;

                        var isModPb = !progressions.Any(x => x.Type == ProgressionType.Altitude && !string.IsNullOrWhiteSpace(x.Mods) && x.Mods.Contains(mod) && x.Value > run.Altitude);

                        var modProgression = new Progression()
                        {
                            UserId = user.Id,
                            TetrioId = run.TetrioId,
                            Value = run.Altitude,
                            Type = ProgressionType.Altitude,
                            Mods = string.IsNullOrWhiteSpace(run.Mods) ? null : run.Mods,
                            PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                            IsPersonalBest = isModPb,
                        };

                        if (!isModPb) continue;

                        progressions.Add(modProgression);
                        context.Add(modProgression);
                    }
                }

                var runSplits = await context.ZenithSplits.AsNoTracking().FirstOrDefaultAsync(x => x.User.Id == user.Id && x.TetrioId == run.TetrioId);

                if (runSplits == null) continue;

                if (string.IsNullOrWhiteSpace(run.Mods))
                {
                    var isHotelPb = runSplits.HotelReachedAt > 0 && !progressions.Any(x => x.Type == ProgressionType.ZenithSplit && x.Value > runSplits.HotelReachedAt && string.IsNullOrWhiteSpace(x.Mods));
                    var isCasinoPb = runSplits.CasinoReachedAt > 0 && !progressions.Any(x => x.Type == ProgressionType.ZenithSplit && x.Value > runSplits.CasinoReachedAt && string.IsNullOrWhiteSpace(x.Mods));
                    var isArenaPb = runSplits.ArenaReachedAt > 0 && !progressions.Any(x => x.Type == ProgressionType.ZenithSplit && x.Value > runSplits.ArenaReachedAt && string.IsNullOrWhiteSpace(x.Mods));
                    var isMuseumPb = runSplits.MuseumReachedAt > 0 && !progressions.Any(x => x.Type == ProgressionType.ZenithSplit && x.Value > runSplits.MuseumReachedAt && string.IsNullOrWhiteSpace(x.Mods));
                    var isOfficesPb = runSplits.OfficesReachedAt > 0 && !progressions.Any(x => x.Type == ProgressionType.ZenithSplit && x.Value > runSplits.OfficesReachedAt && string.IsNullOrWhiteSpace(x.Mods));
                    var isLaboratoryPb = runSplits.LaboratoryReachedAt > 0 && !progressions.Any(x => x.Type == ProgressionType.ZenithSplit && x.Value > runSplits.LaboratoryReachedAt && string.IsNullOrWhiteSpace(x.Mods));
                    var isCorePb = runSplits.CoreReachedAt > 0 && !progressions.Any(x => x.Type == ProgressionType.ZenithSplit && x.Value > runSplits.CoreReachedAt && string.IsNullOrWhiteSpace(x.Mods));
                    var isCorruptionPb = runSplits.CorruptionReachedAt > 0 && !progressions.Any(x => x.Type == ProgressionType.ZenithSplit && x.Value > runSplits.CorruptionReachedAt && string.IsNullOrWhiteSpace(x.Mods));
                    var isPotgPb = runSplits.PlatformOfTheGodsReachedAt > 0 && !progressions.Any(x => x.Type == ProgressionType.ZenithSplit && x.Value > runSplits.PlatformOfTheGodsReachedAt && string.IsNullOrWhiteSpace(x.Mods));

                    if(isHotelPb)
                    {
                        var splitProgression = new Progression()
                        {
                            UserId = user.Id,
                            TetrioId = run.TetrioId,
                            Type = ProgressionType.ZenithSplit,
                            Value = runSplits.HotelReachedAt,
                            PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                            IsPersonalBest = isHotelPb,
                            Floor = ZenithFloor.Hotel,
                        };

                        progressions.Add(splitProgression);
                        context.Add(splitProgression);
                    }

                    if(isCasinoPb)
                    {
                        var splitProgression = new Progression()
                        {
                            UserId = user.Id,
                            TetrioId = run.TetrioId,
                            Type = ProgressionType.ZenithSplit,
                            Value = runSplits.CasinoReachedAt,
                            PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                            IsPersonalBest = isHotelPb,
                            Floor = ZenithFloor.Casino,
                        };

                        progressions.Add(splitProgression);
                        context.Add(splitProgression);
                    }

                    if(isArenaPb)
                    {
                        var splitProgression = new Progression()
                        {
                            UserId = user.Id,
                            TetrioId = run.TetrioId,
                            Type = ProgressionType.ZenithSplit,
                            Value = runSplits.ArenaReachedAt,
                            PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                            IsPersonalBest = isArenaPb,
                            Floor = ZenithFloor.Arena,
                        };

                        progressions.Add(splitProgression);
                        context.Add(splitProgression);
                    }

                    if(isMuseumPb)
                    {
                        var splitProgression = new Progression()
                        {
                            UserId = user.Id,
                            TetrioId = run.TetrioId,
                            Type = ProgressionType.ZenithSplit,
                            Value = runSplits.MuseumReachedAt,
                            PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                            IsPersonalBest = isMuseumPb,
                            Floor = ZenithFloor.Museum,
                        };

                        progressions.Add(splitProgression);
                        context.Add(splitProgression);
                    }

                    if(isOfficesPb)
                    {
                        var splitProgression = new Progression()
                        {
                            UserId = user.Id,
                            TetrioId = run.TetrioId,
                            Type = ProgressionType.ZenithSplit,
                            Value = runSplits.OfficesReachedAt,
                            PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                            IsPersonalBest = isOfficesPb,
                            Floor = ZenithFloor.Offices,
                        };

                        progressions.Add(splitProgression);
                        context.Add(splitProgression);
                    }

                    if(isLaboratoryPb)
                    {
                        var splitProgression = new Progression()
                        {
                            UserId = user.Id,
                            TetrioId = run.TetrioId,
                            Type = ProgressionType.ZenithSplit,
                            Value = runSplits.LaboratoryReachedAt,
                            PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                            IsPersonalBest = isLaboratoryPb,
                            Floor = ZenithFloor.Laboratory,
                        };

                        progressions.Add(splitProgression);
                        context.Add(splitProgression);
                    }

                    if(isCorePb)
                    {
                        var splitProgression = new Progression()
                        {
                            UserId = user.Id,
                            TetrioId = run.TetrioId,
                            Type = ProgressionType.ZenithSplit,
                            Value = runSplits.CoreReachedAt,
                            PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                            IsPersonalBest = isCorePb,
                            Floor = ZenithFloor.Core,
                        };

                        progressions.Add(splitProgression);
                        context.Add(splitProgression);
                    }

                    if(isCorruptionPb)
                    {
                        var splitProgression = new Progression()
                        {
                            UserId = user.Id,
                            TetrioId = run.TetrioId,
                            Type = ProgressionType.ZenithSplit,
                            Value = runSplits.CorruptionReachedAt,
                            PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                            IsPersonalBest = isCorruptionPb,
                            Floor = ZenithFloor.Corruption,
                        };

                        progressions.Add(splitProgression);
                        context.Add(splitProgression);
                    }

                    if(isPotgPb)
                    {
                        var splitProgression = new Progression()
                        {
                            UserId = user.Id,
                            TetrioId = run.TetrioId,
                            Type = ProgressionType.ZenithSplit,
                            Value = runSplits.PlatformOfTheGodsReachedAt,
                            PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                            IsPersonalBest = isPotgPb,
                            Floor = ZenithFloor.PlatformOfTheGods,
                        };

                        progressions.Add(splitProgression);
                        context.Add(splitProgression);
                    }
                }
                else
                {
                    var mods = run.Mods.Split(' ');

                    foreach (var mod in mods)
                    {
                        if(mod.Contains("snowman") || mod.Contains("pento")) continue;

                        // Hotel mod PBs
                        if (runSplits.HotelReachedAt > 0)
                        {
                            var isHotelModPb = !progressions.Any(x => x.Type == ProgressionType.ZenithSplit
                                && x.Floor == ZenithFloor.Hotel
                                && !string.IsNullOrWhiteSpace(x.Mods)
                                && x.Mods.Contains(mod)
                                && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento"))
                                && x.Value < runSplits.HotelReachedAt);

                            if (isHotelModPb)
                            {
                                var splitProgression = new Progression()
                                {
                                    UserId = user.Id,
                                    TetrioId = run.TetrioId,
                                    Type = ProgressionType.ZenithSplit,
                                    Value = runSplits.HotelReachedAt,
                                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                                    IsPersonalBest = isHotelModPb,
                                    Floor = ZenithFloor.Hotel,
                                    Mods = run.Mods
                                };

                                progressions.Add(splitProgression);
                                context.Add(splitProgression);
                            }
                        }

                        // Casino mod PBs
                        if (runSplits.CasinoReachedAt > 0)
                        {
                            var isCasinoModPb = !progressions.Any(x => x.Type == ProgressionType.ZenithSplit
                                && x.Floor == ZenithFloor.Casino
                                && !string.IsNullOrWhiteSpace(x.Mods)
                                && x.Mods.Contains(mod)
                                && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento"))
                                && x.Value < runSplits.CasinoReachedAt);

                            if (isCasinoModPb)
                            {
                                var splitProgression = new Progression()
                                {
                                    UserId = user.Id,
                                    TetrioId = run.TetrioId,
                                    Type = ProgressionType.ZenithSplit,
                                    Value = runSplits.CasinoReachedAt,
                                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                                    IsPersonalBest = isCasinoModPb,
                                    Floor = ZenithFloor.Casino,
                                    Mods = run.Mods
                                };

                                progressions.Add(splitProgression);
                                context.Add(splitProgression);
                            }
                        }

                        // Arena mod PBs
                        if (runSplits.ArenaReachedAt > 0)
                        {
                            var isArenaModPb = !progressions.Any(x => x.Type == ProgressionType.ZenithSplit
                                && x.Floor == ZenithFloor.Arena
                                && !string.IsNullOrWhiteSpace(x.Mods)
                                && x.Mods.Contains(mod)
                                && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento"))
                                && x.Value < runSplits.ArenaReachedAt);

                            if (isArenaModPb)
                            {
                                var splitProgression = new Progression()
                                {
                                    UserId = user.Id,
                                    TetrioId = run.TetrioId,
                                    Type = ProgressionType.ZenithSplit,
                                    Value = runSplits.ArenaReachedAt,
                                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                                    IsPersonalBest = isArenaModPb,
                                    Floor = ZenithFloor.Arena,
                                    Mods = run.Mods
                                };

                                progressions.Add(splitProgression);
                                context.Add(splitProgression);
                            }
                        }

                        // Museum mod PBs
                        if (runSplits.MuseumReachedAt > 0)
                        {
                            var isMuseumModPb = !progressions.Any(x => x.Type == ProgressionType.ZenithSplit
                                && x.Floor == ZenithFloor.Museum
                                && !string.IsNullOrWhiteSpace(x.Mods)
                                && x.Mods.Contains(mod)
                                && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento"))
                                && x.Value < runSplits.MuseumReachedAt);

                            if (isMuseumModPb)
                            {
                                var splitProgression = new Progression()
                                {
                                    UserId = user.Id,
                                    TetrioId = run.TetrioId,
                                    Type = ProgressionType.ZenithSplit,
                                    Value = runSplits.MuseumReachedAt,
                                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                                    IsPersonalBest = isMuseumModPb,
                                    Floor = ZenithFloor.Museum,
                                    Mods = run.Mods
                                };

                                progressions.Add(splitProgression);
                                context.Add(splitProgression);
                            }
                        }

                        // Offices mod PBs
                        if (runSplits.OfficesReachedAt > 0)
                        {
                            var isOfficesModPb = !progressions.Any(x => x.Type == ProgressionType.ZenithSplit
                                && x.Floor == ZenithFloor.Offices
                                && !string.IsNullOrWhiteSpace(x.Mods)
                                && x.Mods.Contains(mod)
                                && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento"))
                                && x.Value < runSplits.OfficesReachedAt);

                            if (isOfficesModPb)
                            {
                                var splitProgression = new Progression()
                                {
                                    UserId = user.Id,
                                    TetrioId = run.TetrioId,
                                    Type = ProgressionType.ZenithSplit,
                                    Value = runSplits.OfficesReachedAt,
                                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                                    IsPersonalBest = isOfficesModPb,
                                    Floor = ZenithFloor.Offices,
                                    Mods = run.Mods
                                };

                                progressions.Add(splitProgression);
                                context.Add(splitProgression);
                            }
                        }

                        // Laboratory mod PBs
                        if (runSplits.LaboratoryReachedAt > 0)
                        {
                            var isLaboratoryModPb = !progressions.Any(x => x.Type == ProgressionType.ZenithSplit
                                && x.Floor == ZenithFloor.Laboratory
                                && !string.IsNullOrWhiteSpace(x.Mods)
                                && x.Mods.Contains(mod)
                                && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento"))
                                && x.Value < runSplits.LaboratoryReachedAt);

                            if (isLaboratoryModPb)
                            {
                                var splitProgression = new Progression()
                                {
                                    UserId = user.Id,
                                    TetrioId = run.TetrioId,
                                    Type = ProgressionType.ZenithSplit,
                                    Value = runSplits.LaboratoryReachedAt,
                                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                                    IsPersonalBest = isLaboratoryModPb,
                                    Floor = ZenithFloor.Laboratory,
                                    Mods = run.Mods
                                };

                                progressions.Add(splitProgression);
                                context.Add(splitProgression);
                            }
                        }

                        // Core mod PBs
                        if (runSplits.CoreReachedAt > 0)
                        {
                            var isCoreModPb = !progressions.Any(x => x.Type == ProgressionType.ZenithSplit
                                && x.Floor == ZenithFloor.Core
                                && !string.IsNullOrWhiteSpace(x.Mods)
                                && x.Mods.Contains(mod)
                                && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento"))
                                && x.Value < runSplits.CoreReachedAt);

                            if (isCoreModPb)
                            {
                                var splitProgression = new Progression()
                                {
                                    UserId = user.Id,
                                    TetrioId = run.TetrioId,
                                    Type = ProgressionType.ZenithSplit,
                                    Value = runSplits.CoreReachedAt,
                                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                                    IsPersonalBest = isCoreModPb,
                                    Floor = ZenithFloor.Core,
                                    Mods = run.Mods
                                };

                                progressions.Add(splitProgression);
                                context.Add(splitProgression);
                            }
                        }

                        // Corruption mod PBs
                        if (runSplits.CorruptionReachedAt > 0)
                        {
                            var isCorruptionModPb = !progressions.Any(x => x.Type == ProgressionType.ZenithSplit
                                && x.Floor == ZenithFloor.Corruption
                                && !string.IsNullOrWhiteSpace(x.Mods)
                                && x.Mods.Contains(mod)
                                && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento"))
                                && x.Value < runSplits.CorruptionReachedAt);

                            if (isCorruptionModPb)
                            {
                                var splitProgression = new Progression()
                                {
                                    UserId = user.Id,
                                    TetrioId = run.TetrioId,
                                    Type = ProgressionType.ZenithSplit,
                                    Value = runSplits.CorruptionReachedAt,
                                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                                    IsPersonalBest = isCorruptionModPb,
                                    Floor = ZenithFloor.Corruption,
                                    Mods = run.Mods
                                };

                                progressions.Add(splitProgression);
                                context.Add(splitProgression);
                            }
                        }

                        // PlatformOfTheGods mod PBs
                        if (runSplits.PlatformOfTheGodsReachedAt > 0)
                        {
                            var isPotgModPb = !progressions.Any(x => x.Type == ProgressionType.ZenithSplit
                                && x.Floor == ZenithFloor.PlatformOfTheGods
                                && !string.IsNullOrWhiteSpace(x.Mods)
                                && x.Mods.Contains(mod)
                                && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento"))
                                && x.Value < runSplits.PlatformOfTheGodsReachedAt);

                            if (isPotgModPb)
                            {
                                var splitProgression = new Progression()
                                {
                                    UserId = user.Id,
                                    TetrioId = run.TetrioId,
                                    Type = ProgressionType.ZenithSplit,
                                    Value = runSplits.PlatformOfTheGodsReachedAt,
                                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                                    IsPersonalBest = isPotgModPb,
                                    Floor = ZenithFloor.PlatformOfTheGods,
                                    Mods = run.Mods
                                };

                                progressions.Add(splitProgression);
                                context.Add(splitProgression);
                            }
                        }
                    }
                }
            }

            sw.Stop();

            totalProgressions += progressions.Count;

            Console.WriteLine($"[PROGRESSIONS] {progressions.Count} progressions for {user.Username} | Took: {sw.ElapsedMilliseconds}ms");
        }

        Console.WriteLine($"Saving...");

        var c = await context.SaveChangesAsync();

        return Ok(totalProgressions);
    }
    #endif
}