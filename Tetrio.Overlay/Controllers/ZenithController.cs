using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TetraLeague.Overlay.Generator;
using TetraLeague.Overlay.Network.Api;
using TetraLeague.Overlay.Network.Api.Models;

namespace TetraLeague.Overlay.Controllers;

public class ZenithController : BaseController
{
    public ZenithController(TetrioApi api) : base(api) { }

    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("This Endpoint is for Quick Play Overlays");
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

        var stats = await _api.GetUserSummaries(username);

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
        var stats = await _api.GetRecentZenithRecords(username, expert);
        var careerBest = await _api.GetZenithStats(username, expert);

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