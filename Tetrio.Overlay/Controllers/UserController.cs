using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TetraLeague.Overlay.Network.Api.Tetrio;
using TetraLeague.Overlay.Network.Api.Tetrio.Models;
using Tetrio.Overlay.Database;
using Tetrio.Overlay.Database.Enums;

namespace TetraLeague.Overlay.Controllers;

public class UserController : BaseController
{
    private readonly TetrioContext _context;

    public UserController(TetrioApi api, TetrioContext context) : base(api)
    {
        this._context = context;
    }

    [HttpGet]
    [Route("{username}/stats")]
    public async Task<ActionResult> Stats(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return NotFound();

        var userData = await Api.GetUserInformation(username);
        var userSummaryData = await Api.GetUserSummaries(username);

        var data = new
        {
            Badges = userData?.Badges?.Select(x => x.Id),
            SummaryData = userSummaryData
        };

        return Ok(data);
    }

    [HttpGet]
    [Route("{username}")]
    public async Task<ActionResult> View(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return NotFound();

        username = username.ToLower();

        var html = await System.IO.File.ReadAllTextAsync("wwwroot/web/user.html");

        html = html.Replace("{username}", username);

        return Content(html, "text/html");
    }

    [HttpGet]
    [Route("{username}/profileData")]
    public async Task<ActionResult> ProfileData(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return NotFound();

        var userInfo = await GetTetrioUserInformation(username);

        if (userInfo == null) return NotFound();

        return Ok(userInfo);
    }

    private async Task<SlimUserInfo?> GetTetrioUserInformation(string username)
    {
        var user = await Api.GetUserInformation(username);

        if(user == default) return null;

        return new SlimUserInfo
        {
            Username = user.Username,
            Avatar = $"https://tetr.io/user-content/avatars/{user.Id}.jpg?rv={user.Avatar}",
            Banner = $"https://tetr.io/user-content/banners/{user.Id}.jpg?rv={user.Banner}",
        };
    }

    [HttpGet]
    [Route("{username}/dailyData")]
    public async Task<ActionResult> DailyData(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return NotFound();

        username = username.ToLower();

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

        if(user == default) return NotFound();

        var aggregateData = await _context.ZenithSplits
            .Where(x => x.User.Id == user.Id)
            .GroupBy(x => x.User.Id)
            .Select(group => new
            {
                SplitAverages = new {
                    Hotel = group.Where(x => x.HotelReachedAt > 0).Average(x => (double?)x.HotelReachedAt) ?? 0,
                    Casino = group.Where(x => x.CasinoReachedAt > 0).Average(x => (double?)x.CasinoReachedAt) ?? 0,
                    Arena = group.Where(x => x.ArenaReachedAt > 0).Average(x => (double?)x.ArenaReachedAt) ?? 0,
                    Museum = group.Where(x => x.MuseumReachedAt > 0).Average(x => (double?)x.MuseumReachedAt) ?? 0,
                    Offices = group.Where(x => x.OfficesReachedAt > 0).Average(x => (double?)x.OfficesReachedAt) ?? 0,
                    Laboratory = group.Where(x => x.LaboratoryReachedAt > 0).Average(x => (double?)x.LaboratoryReachedAt) ?? 0,
                    Core = group.Where(x => x.CoreReachedAt > 0).Average(x => (double?)x.CoreReachedAt) ?? 0,
                    Corruption = group.Where(x => x.CorruptionReachedAt > 0).Average(x => (double?)x.CorruptionReachedAt) ?? 0,
                    PlatformOfTheGods = group.Where(x => x.PlatformOfTheGodsReachedAt > 0).Average(x => (double?)x.PlatformOfTheGodsReachedAt) ?? 0,
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
                    PlatformOfTheGods = group.Where(x => x.PlatformOfTheGodsReachedAt > 0).Min(x => (int?)x.PlatformOfTheGodsReachedAt) ?? 0,
                }
            })
            .SingleOrDefaultAsync();

        var runCount = await _context.Runs.Where(x => x.User.Id == user.Id).CountAsync();
        var splitsCount = await _context.ZenithSplits.Where(x => x.User.Id == user.Id).CountAsync();
        var daysParticipated = await _context.Users.Where(x => x.Username == username).SelectMany(x => x.Challenges).OrderByDescending(x => x.Date).Select(x => x.Date).GroupBy(x => x).CountAsync();

        var challengesCompleted = await _context.Users.SelectMany(x => x.Challenges).CountAsync();

        var userInfo = await GetTetrioUserInformation(username);

        return Ok(new
        {
            UserInfo = userInfo,
            TetrioId = user.TetrioId,
            Runs = runCount,
            Splits = splitsCount,
            ChallengesCompleted = challengesCompleted,
            DaysParticipated = daysParticipated,
            SplitAverages = new
            {
                Hotel = TimeSpan.FromMilliseconds(aggregateData?.SplitAverages.Hotel ?? 0).ToString(@"mm\:ss\.fff"),
                Casino = TimeSpan.FromMilliseconds(aggregateData?.SplitAverages.Casino ?? 0).ToString(@"mm\:ss\.fff"),
                Arena = TimeSpan.FromMilliseconds(aggregateData?.SplitAverages.Arena ?? 0).ToString(@"mm\:ss\.fff"),
                Museum = TimeSpan.FromMilliseconds(aggregateData?.SplitAverages.Museum ?? 0).ToString(@"mm\:ss\.fff"),
                Offices = TimeSpan.FromMilliseconds(aggregateData?.SplitAverages.Offices ?? 0).ToString(@"mm\:ss\.fff"),
                Laboratory = TimeSpan.FromMilliseconds(aggregateData?.SplitAverages.Laboratory ?? 0).ToString(@"mm\:ss\.fff"),
                Core = TimeSpan.FromMilliseconds(aggregateData?.SplitAverages.Core ?? 0).ToString(@"mm\:ss\.fff"),
                Corruption = TimeSpan.FromMilliseconds(aggregateData?.SplitAverages.Corruption ?? 0).ToString(@"mm\:ss\.fff"),
                PlatformOfTheGods = TimeSpan.FromMilliseconds(aggregateData?.SplitAverages.PlatformOfTheGods ?? 0).ToString(@"mm\:ss\.fff")
            },
            GoldSplits =  new
            {
                Hotel = TimeSpan.FromMilliseconds(aggregateData?.GoldSplits.Hotel ?? 0).ToString(@"mm\:ss\.fff"),
                Casino = TimeSpan.FromMilliseconds(aggregateData?.GoldSplits.Casino ?? 0).ToString(@"mm\:ss\.fff"),
                Arena = TimeSpan.FromMilliseconds(aggregateData?.GoldSplits.Arena ?? 0).ToString(@"mm\:ss\.fff"),
                Museum = TimeSpan.FromMilliseconds(aggregateData?.GoldSplits.Museum ?? 0).ToString(@"mm\:ss\.fff"),
                Offices = TimeSpan.FromMilliseconds(aggregateData?.GoldSplits.Offices ?? 0).ToString(@"mm\:ss\.fff"),
                Laboratory = TimeSpan.FromMilliseconds(aggregateData?.GoldSplits.Laboratory ?? 0).ToString(@"mm\:ss\.fff"),
                Core = TimeSpan.FromMilliseconds(aggregateData?.GoldSplits.Core ?? 0).ToString(@"mm\:ss\.fff"),
                Corruption = TimeSpan.FromMilliseconds(aggregateData?.GoldSplits.Corruption ?? 0).ToString(@"mm\:ss\.fff"),
                PlatformOfTheGods = TimeSpan.FromMilliseconds(aggregateData?.GoldSplits.PlatformOfTheGods ?? 0).ToString(@"mm\:ss\.fff")
            },
        });
    }

    [HttpGet]
    [Route("{username}/runs")]
    public async Task<ActionResult> GetRuns(string? username, int page = 0, int pageSize = 25)
    {
        if (string.IsNullOrWhiteSpace(username)) return NotFound();

        var runs = await _context.Runs.Where(x => x.User.Username == username).OrderByDescending(x => x.PlayedAt).ThenByDescending(x => x.CreatedAt).Skip(page * pageSize).Take(pageSize).Select(x => new
        {
            TetrioId = x.TetrioId,
            Mods = x.Mods,
            Altitude = Math.Round(x.Altitude, 2),
            Quads = x.Quads,
            Spins = x.Spins,
            AllClears = x.AllClears,
            KOs = x.KOs,
            Apm = Math.Round(x.Apm, 2),
            Pps = Math.Round(x.Pps, 2),
            Vs = Math.Round(x.Vs, 2),
            Finesse = Math.Round(x.Finesse,2),
        }).ToArrayAsync();

        return Ok(runs);
    }

    [HttpGet]
    [Route("{username}/challenges")]
    public async Task<ActionResult> GetChallenges(string? username, int page = 0, int pageSize = 25)
    {
        if (string.IsNullOrWhiteSpace(username)) return NotFound();

        var runs = await _context.Users.Where(x => x.Username == username).SelectMany(x => x.Challenges).OrderByDescending(x => x.Date).ThenByDescending(x => x.Points).Skip(page * pageSize).Take(pageSize).Select(x => new
        {
            Date = x.Date,
            Difficulty = x.Points,
            Mods = x.Mods,
            Conditions = x.Conditions.Select( a => new
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
        if (string.IsNullOrWhiteSpace(username)) return NotFound();

        var runs = await _context.Users.Where(x => x.Username == username).SelectMany(x => x.Challenges).OrderByDescending(x => x.Date).ThenByDescending(x => x.Points).Skip(page * pageSize).Take(pageSize).Select(x => new
        {
            Date = x.Date,
            Difficulty = x.Points,
            Mods = x.Mods,
            Conditions = x.Conditions.Select( a => new
            {
                a.ChallengeId,
                a.Type,
                a.Value
            })
        }).GroupBy(x => x.Date).ToArrayAsync();

        var a = runs.Select(x =>
        {
            var date = x.Key;

            var veryEasyCompleted = false;
            var easyCompleted = false;
            var normalCompleted = false;
            var hardCompleted = false;
            var expertCompleted = false;

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
            };
        }).OrderByDescending(x => x.Date).ToArray();

        return Ok(a);
    }
}