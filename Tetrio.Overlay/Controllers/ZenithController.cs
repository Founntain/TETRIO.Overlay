﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TetraLeague.Overlay.Network.Api.Tetrio;
using TetraLeague.Overlay.Network.Api.Tetrio.Models;
using Tetrio.Overlay.Database;
using Tetrio.Overlay.Database.Entities;

namespace TetraLeague.Overlay.Controllers;

public class ZenithController : BaseController
{
    private readonly TetrioContext _context;

    public ZenithController(TetrioApi api, TetrioContext context) : base(api)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("This Endpoint is for Quick Play Overlays");
    }

    [HttpGet]
    [Route("daily/generate")]
    public async Task<IActionResult> GenerateDailyChallenges()
    {
        var generator = new ChallengeGenerator();

        var day = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);

        var challengesExist = await _context.Challenges.AnyAsync(x => x.Date == day);

        if (challengesExist)
        {
            return Ok("Daily Challenge already exist for this day");
        }

        var challenges = await generator.GenerateChallengesForDay(_context);

        await _context.AddRangeAsync(challenges);
        await _context.SaveChangesAsync();

        return Ok(challenges.Select(x => x.Conditions?.Select(y => y.ToString())));
    }

    [HttpGet]
    [Route("daily")]
    public async Task<IActionResult> GetDailyChallenges(ulong discordId = 0)
    {
        var day = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);

        var challenges = await _context.Challenges.Where(x => x.Date == day).OrderByDescending(x => x.Points).ToListAsync();

        if (challenges.Count == 0)
        {
            await GenerateDailyChallenges();

            challenges = await _context.Challenges.Where(x => x.Date == day).OrderByDescending(x => x.Points).ToListAsync();
        }

        return Ok(challenges);
    }

    [HttpPost]
    [Route("daily/submit")]
    public async Task<IActionResult> SubmitDailyChallenge()
    {
        return StatusCode(501);

        var authResult = await CheckIfAuthorized(_context);

        if (authResult is not OkResult and not OkObjectResult)
        {
            return authResult;
        }

        if (authResult is not OkObjectResult result) return Ok("You are allowed to submit daily challenges");

        var user = result.Value as User;

        if (user == null)
        {
            return Ok("You are not authorized to submit daily challenges, please log in again and try again");
        }

        var records = await Api.GetRecentZenithRecords(user.Username, false, 10);
        var expertRecords = await Api.GetRecentZenithRecords(user.Username, true, 10);

        if (records == null || expertRecords == null)
        {
            return Ok("Could not fetch your recent records, please try again later");
        }

        var allRecords = new List<Record>();

        allRecords.AddRange(records.Entries);
        allRecords.AddRange(expertRecords.Entries);

        return Ok("You are allowed to submit daily challenges");
    }

    [HttpGet]
    [Route("{username}")]
    public async Task<ActionResult> Web(string username)
    {
        username = username.ToLower();

        var html = await System.IO.File.ReadAllTextAsync("wwwroot/web/zenith.html");

        html = html.Replace("{username}", username);

        return Content(html, "text/html");
    }

    [HttpGet]
    [Route("splits/{username}")]
    public async Task<ActionResult> WebTest(string username, bool expert = false)
    {
        username = username.ToLower();

        var html = await System.IO.File.ReadAllTextAsync("wwwroot/web/splits.html");

        html = html.Replace("{username}", username);
        html = html.Replace("{expert}", expert.ToString());

        return Content(html, "text/html");
    }

    [HttpGet]
    [Route("{username}/stats")]
    public async Task<ActionResult> GetStats(string username)
    {
        username = username.ToLower();

        var stats = await Api.GetUserSummaries(username);

        if (stats.Zenith.Record == null)
        {
            stats.Zenith.Record = stats.Zenith.Best.Record;
        }

        if (stats.ZenithExpert.Record == null)
        {
            stats.ZenithExpert.Record = stats.ZenithExpert.Best.Record;
        }

        var expertPlayed = stats.ZenithExpert.Record != null && stats.ZenithExpert.Best.Record != null;

        return Ok(new
        {
            Zenith = new
            {
                Altitude = stats.Zenith.Record?.Results.Stats.Zenith.Altitude,
                Best = stats.Zenith.Best?.Record?.Results.Stats.Zenith.Altitude,

                Pps = stats.Zenith.Record?.Results.Aggregatestats.Pps,
                Apm = stats.Zenith.Record?.Results.Aggregatestats.Apm,
                Vs = stats.Zenith.Record?.Results.Aggregatestats.Vsscore,

                Mods = stats.Zenith.Record?.Extras.Zenith.Mods
            },
            ZenithExpert = new {
                Altitude = stats.ZenithExpert?.Record?.Results.Stats.Zenith.Altitude,
                Best = stats.ZenithExpert?.Best?.Record?.Results.Stats.Zenith.Altitude,

                Pps = stats.ZenithExpert?.Record?.Results.Aggregatestats.Pps,
                Apm = stats.ZenithExpert?.Record?.Results.Aggregatestats.Apm,
                Vs = stats.ZenithExpert?.Record?.Results.Aggregatestats.Vsscore,

                Mods = stats.ZenithExpert?.Record?.Extras.Zenith.Mods
            },
            ExpertPlayed = expertPlayed
        });
    }

    [HttpGet]
    [Route("splits/{username}/stats")]
    public async Task<ActionResult> GetSplitStats(string username, bool expert = false)
    {
        var stats = await Api.GetRecentZenithRecords(username, expert);
        var careerBest = await Api.GetZenithStats(username, expert);

        var goldSplits = new int[9];
        var secondGoldSplit = new double[9];

        foreach (var entry in stats.Entries)
        {
            List<double?> splits = entry.Results.Stats.Zenith.Splits;

            for (var i = 0; i < splits.Count; i++)
            {
                var split = splits[i];

                if(split == null) continue;

                if (goldSplits[i] == 0)
                {
                    goldSplits[i] = (int) split;

                    continue;
                }

                if (goldSplits[i] > split && split != 0)
                {
                    goldSplits[i] = (int) split;
                }
            }
        }

        var avgTimes = new double[9];

        for (int i = 0; i < goldSplits.Length; i++)
        {
            var iSplits = stats.Entries.Select(x => x.Results.Stats.Zenith.Splits[i]);

            var orderedSplits = iSplits.Where(y => y > 0).Order().ToArray();

            avgTimes[i] = orderedSplits.Average() ?? 0;

            var x = orderedSplits.Length < 2 ? 0 : orderedSplits[1];

            secondGoldSplit[i] = x ?? 0;
        }

        var result = new List<dynamic>();

        var recentSplits = stats.Entries.First().Results.Stats.Zenith.Splits.Select(x => (int) (x ?? 0)).ToArray();
        var careerBestSplits = careerBest.Best!.Record!.Results.Stats.Zenith.Splits.Select(x => (int) (x ?? 0)).ToArray();

        var notReached = false;

        var floorColors = new[]
        {
            "AAfde692",
            "AAffc788",
            "AAffb7c2",
            "AAffba43",
            "AAff917b",
            "AA00ddff",
            "AAff006f",
            "AA98ffb2",
            "AAd677ff",
        };

        var floorNames = new[]
        {
            "HOTEL",
            "CASINO",
            "ARENA",
            "MUSEUM",
            "OFFICES",
            "LABORATORY",
            "THE CORE",
            "CORRUPTION",
            "POTG",
        };

        for (var i = 0; i < goldSplits.Length; i++)
        {

            var split = goldSplits[i];
            var secondSplit = secondGoldSplit[i];
            var isRecentSplitsEmpty = recentSplits.All(y => y == 0);
            var differenceToGold = recentSplits[i] - split;
            var differenceToSecondGold = secondGoldSplit[i] - split;
            var timeDifferenceToGold = recentSplits[i] == 0 ? TimeSpan.FromMilliseconds(0) : TimeSpan.FromMilliseconds(recentSplits[i] - split);
            var timeDifferenceToSecondGold = recentSplits[i] == 0 ? TimeSpan.FromMilliseconds(0) : TimeSpan.FromMilliseconds(secondGoldSplit[i] - split);

            if (isRecentSplitsEmpty)
            {
                if (careerBestSplits.Any(x => x != 0))
                {
                    isRecentSplitsEmpty = false;
                }

                timeDifferenceToGold = TimeSpan.FromMilliseconds(careerBestSplits[i] - split);
            }

            var isSlower = timeDifferenceToGold.TotalMilliseconds > 0;

            if ((timeDifferenceToGold.TotalMilliseconds == 0 && ((timeDifferenceToGold.TotalMilliseconds != 0 || split == 0 || recentSplits[i] == 0) || isRecentSplitsEmpty)) && !notReached)
            {
                notReached = true;
            }

            var o = new
            {
                Floor = floorNames[i],
                FloorColor = floorColors[i],
                Split = split,
                SplitTime = TimeSpan.FromMilliseconds(split).ToString(@"m\:ss\.fff"),
                SecondSplit = secondSplit,
                IsRecentSplitEmpty = isRecentSplitsEmpty,
                DifferenceToGold = differenceToGold,
                DifferenceToSecondGold = differenceToSecondGold,
                TimeDifferenceToGold = timeDifferenceToGold.ToString(@"ss\.fff"),
                TimeDifferenceToSecondGold = timeDifferenceToSecondGold.ToString(@"ss\.fff"),
                IsSlower = isSlower,
                NotReached = notReached,
                AvgTime = TimeSpan.FromMilliseconds(avgTimes[i]),
            };

            result.Add(o);
        }

        return Ok(result);
    }

}