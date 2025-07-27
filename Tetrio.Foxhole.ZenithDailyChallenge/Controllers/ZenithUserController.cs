using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Enums;
using Tetrio.Foxhole.Network.Api.Tetrio;

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
    [Route("{username}/daily")]
    public async Task<ActionResult> GetDailyData(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        username = username.ToLower();

        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

        if(user == default) return NotFound();

        var splitData = await context.ZenithSplits
            .AsNoTracking()
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
            }).SingleOrDefaultAsync();

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
        }).FirstOrDefaultAsync();

        var runCount = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).CountAsync();
        var splitsCount = await context.ZenithSplits.AsNoTracking().Where(x => x.User.Id == user.Id).CountAsync();
        var daysParticipated = await context.Users.AsNoTracking().Where(x => x.Username == username).SelectMany(x => x.Challenges).OrderByDescending(x => x.Date).Select(x => x.Date).GroupBy(x => x).CountAsync();

        var totalChallengesCompleted = await context.Users.AsNoTracking().Where(x => x.Username == username).SelectMany(x => x.Challenges).CountAsync();
        var challengesCompleted = await context.Users.AsNoTracking().Where(x => x.Username == username).SelectMany(x => x.Challenges).GroupBy(x => x.Date).CountAsync();

        var userInfo = await GetTetrioUserInformation(username);

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
        }

        var scores = await context.Users.AsNoTracking().Where(x => x.Username == username).Select(x => new
        {

            NormalScore = x.Challenges.Where(y => y.Points != (byte)Difficulty.Expert && y.Points != (byte)Difficulty.Reverse).Sum(y => y.Points),
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
        }).FirstOrDefaultAsync();

        return Ok(new
        {
            UserInfo = userInfo,
            TetrioId = user.TetrioId,
            Runs = runCount,
            Splits = splitsCount,
            ChallengesCompleted = challengesCompleted,
            TotalChallengesCompleted = totalChallengesCompleted,
            DaysParticipated = daysParticipated,
            Altitudes = altitudes,
            MasteryCompletions = masteryCompletions,
            Score = scores == null ? 0 : Math.Round(scores.NormalScore + scores.ExpertScore + ( scores.MasteryScore * 2 )  + (scores.ReverseScore / 2d), 0),
            SplitAverages = new
            {
                Hotel = TimeSpan.FromMilliseconds(splitData?.SplitAverages.Hotel ?? 0).ToString(@"mm\:ss\.fff"),
                Casino = TimeSpan.FromMilliseconds(splitData?.SplitAverages.Casino ?? 0).ToString(@"mm\:ss\.fff"),
                Arena = TimeSpan.FromMilliseconds(splitData?.SplitAverages.Arena ?? 0).ToString(@"mm\:ss\.fff"),
                Museum = TimeSpan.FromMilliseconds(splitData?.SplitAverages.Museum ?? 0).ToString(@"mm\:ss\.fff"),
                Offices = TimeSpan.FromMilliseconds(splitData?.SplitAverages.Offices ?? 0).ToString(@"mm\:ss\.fff"),
                Laboratory = TimeSpan.FromMilliseconds(splitData?.SplitAverages.Laboratory ?? 0).ToString(@"mm\:ss\.fff"),
                Core = TimeSpan.FromMilliseconds(splitData?.SplitAverages.Core ?? 0).ToString(@"mm\:ss\.fff"),
                Corruption = TimeSpan.FromMilliseconds(splitData?.SplitAverages.Corruption ?? 0).ToString(@"mm\:ss\.fff"),
                PlatformOfTheGods = TimeSpan.FromMilliseconds(splitData?.SplitAverages.PlatformOfTheGods ?? 0).ToString(@"mm\:ss\.fff")
            },
            GoldSplits =  new
            {
                Hotel = TimeSpan.FromMilliseconds(splitData?.GoldSplits.Hotel ?? 0).ToString(@"mm\:ss\.fff"),
                Casino = TimeSpan.FromMilliseconds(splitData?.GoldSplits.Casino ?? 0).ToString(@"mm\:ss\.fff"),
                Arena = TimeSpan.FromMilliseconds(splitData?.GoldSplits.Arena ?? 0).ToString(@"mm\:ss\.fff"),
                Museum = TimeSpan.FromMilliseconds(splitData?.GoldSplits.Museum ?? 0).ToString(@"mm\:ss\.fff"),
                Offices = TimeSpan.FromMilliseconds(splitData?.GoldSplits.Offices ?? 0).ToString(@"mm\:ss\.fff"),
                Laboratory = TimeSpan.FromMilliseconds(splitData?.GoldSplits.Laboratory ?? 0).ToString(@"mm\:ss\.fff"),
                Core = TimeSpan.FromMilliseconds(splitData?.GoldSplits.Core ?? 0).ToString(@"mm\:ss\.fff"),
                Corruption = TimeSpan.FromMilliseconds(splitData?.GoldSplits.Corruption ?? 0).ToString(@"mm\:ss\.fff"),
                PlatformOfTheGods = TimeSpan.FromMilliseconds(splitData?.GoldSplits.PlatformOfTheGods ?? 0).ToString(@"mm\:ss\.fff")
            },
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
                    Back2Back = x.Back2Back,
                    SpeedrunSeen = x.SpeedrunSeen,
                    SpeedrunCompleted = x.SpeedrunCompleted
                }).ToArrayAsync();

        return Ok(runs);
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
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        var runs = (await context.Users
            .AsNoTracking()
            .Where(x => x.Username == username)
            .SelectMany(x => x.Challenges)
            .Select(x => new
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
                })
            .GroupBy(x => x.Date)
            .ToArrayAsync())
            .OrderByDescending(x => x.Key)
            .Skip( page * pageSize).Take(pageSize);

        context.ChangeTracker.LazyLoadingEnabled = true;

        var a = runs.Select(x =>
        {
            var date = x.Key;

            var veryEasyCompleted = false;
            var easyCompleted = false;
            var normalCompleted = false;
            var hardCompleted = false;
            var expertCompleted = false;
            var reverseCompleted = false;

            foreach (var challenge in x)
            {
                switch ((Difficulty) challenge.Difficulty)
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
            };
        }).OrderByDescending(x => x.Date).ToArray();

        return Ok(a);
    }

    [HttpGet]
    [Route("{username}/getTodaysChallengeCompletions")]
    public async Task<ActionResult> GetTodaysChallengeCompletions(string username)
    {
        username = username.ToLower();

        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

        if (user == null) return Ok("You are not authorized to submit daily challenges, please log in again and try again");
        if (user.IsRestricted) return BadRequest("No bad person allowed, no submitting for you, ask founntain to unrestrict you");

        var utc = DateTime.UtcNow;

        var date = new DateOnly(utc.Year, utc.Month, utc.Day);

        var challenges = await context.Users
            .AsNoTracking()
            .Where(x => x.Username == username)
            .SelectMany(x => x.Challenges)
            .Where(x => x.Date == date)
            .Select(x => new
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

        var veryEasyCompleted = false;
        var easyCompleted = false;
        var normalCompleted = false;
        var hardCompleted = false;
        var expertCompleted = false;
        var reverseCompleted = false;

        foreach (var challenge in challenges)
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
                x.AllSpinCompleted
            }).FirstOrDefaultAsync();

        return Ok(new
        {
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

        if(user == null) return NotFound();

        var contributions = await context.CommunityContributions
            .AsNoTracking()
            .OrderByDescending(x => x.CommunityChallenge.StartDate)
            .Where(x => x.UserId == user.Id && !x.IsLate)
            .GroupBy(x => x.CommunityChallengeId)
            .Skip(page * pageSize).Take(pageSize)
            .Select(group => new
                {
                    Date = group.First().CommunityChallenge.StartDate,
                    Challenge = $"{group.First().CommunityChallenge.StartDate:yyyy-MM-dd}",
                    TotalAmountContributed = Math.Round(group.Sum(x => x.Amount), 2),
                    ConditionType = group.First().CommunityChallenge.ConditionType,
                }).ToArrayAsync();

        var contributionsCount = await context.CommunityContributions
            .AsNoTracking()
            .OrderByDescending(x => x.CommunityChallenge.StartDate)
            .Where(x => x.UserId == user.Id && !x.IsLate)
            .GroupBy(x => x.CommunityChallengeId).CountAsync();

        var returnValue = contributions.OrderByDescending(x => x.Date).Select(x => new
        {
            Date = x.Date,
            Challenge = x.Challenge,
            TotalAmountContributed = x.TotalAmountContributed,
            ConditionType = x.ConditionType,
            TotalContributions = contributionsCount
        });

        return Ok(returnValue);
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
}