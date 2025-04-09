using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TetraLeague.Overlay.Network.Api.Tetrio;
using TetraLeague.Overlay.Network.Api.Tetrio.Models;
using Tetrio.Overlay.Database;
using Tetrio.Overlay.Database.Entities;
using Tetrio.Zenith.DailyChallenge;

namespace TetraLeague.Overlay.Controllers;

[Route("zenith/daily")]
public class DailyController : BaseController
{
    private readonly TetrioContext _context;

    public DailyController(TetrioApi api, TetrioContext context) : base(api)
    {
        _context = context;
    }


    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetDailyChallenges(ulong discordId = 0)
    {
        var day = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);

        var challengeCount = await _context.Challenges.Where(x => x.Date == day).CountAsync();

        if (challengeCount == 0)
        {
            await GenerateDailyChallenges();
        }

        var challenges = await _context.Challenges.Where(x => x.Date == day).OrderByDescending(x => x.Points).Select(x => new Challenge
        {
            Id = x.Id,
            Conditions = x.Conditions.Select(a => new ChallengeCondition
            {
                Id = a.Id,
                ChallengeId = a.ChallengeId,
                Value = a.Value,
                Type = a.Type,
            }).ToHashSet(),
            Date = x.Date,
            Points = x.Points,
            Mods = x.Mods,
        }).ToListAsync();

        return Ok(challenges);
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

        var challengesExist = await _context.Challenges.AnyAsync(x => x.Date == day);

        if (challengesExist) return Ok("Daily Challenge already exist for this day");

        var challenges = await generator.GenerateChallengesForDay(_context);

        await _context.AddRangeAsync(challenges);
        await _context.SaveChangesAsync();

        return Ok(challenges.Select(x => x.Conditions?.Select(y => y.ToString())));
    }

    [HttpPost]
    [Route("submit")]
    public async Task<IActionResult> SubmitDailyChallenge()
    {
        var authResult = await CheckIfAuthorized(_context);

        if (!authResult.IsAuthorized)
        {
            ResetCookies();

            return StatusCode(authResult.StatusCode, $"{authResult.StatusCode} - Unauthorized. Reason: {authResult.ResponseText}");
        }

        var user = authResult.User;

        if (user == null) return Ok("You are not authorized to submit daily challenges, please log in again and try again");

        var now = DateTime.UtcNow;
        var nextSubmissionPossible = user.LastSubmission?.AddMinutes(1) ?? DateTime.MinValue;

        if(nextSubmissionPossible > DateTime.UtcNow) return BadRequest("Please wait 1 minute before requesting submitting daily challenges again.");

        var day = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);

        var challenges = await _context.Challenges.Where(x => x.Date == day).OrderByDescending(x => x.Points).ToListAsync();

        if (challenges.Count == 0) return Ok("No daily challenges found, submission canceled");

        var records = await Api.GetRecentZenithRecords(user.Username);
        var expertRecords = await Api.GetRecentZenithRecords(user.Username, true);

        if (records == null || expertRecords == null) return Ok("Could not fetch your recent records, please try again later");

        var totalRuns = new List<Record>();

        totalRuns.AddRange(records.Entries);
        totalRuns.AddRange(expertRecords.Entries);

        totalRuns = totalRuns.DistinctBy(x => x.Id).ToList();

        Console.WriteLine($"[DAILY SUBMIT] {user.Username} submitted {totalRuns.Count} run(s)]");

        var tetrioIds = totalRuns.Select(x => x.Id).ToList();

        var existingTetrioIds = new HashSet<string>(await _context.Runs.Where(x => tetrioIds.Contains(x.TetrioId)).Select(x => x.TetrioId).ToListAsync());

        var sw = new Stopwatch();
        sw.Restart();

        var splitsToAdd = new List<ZenithSplit>();
        var runsToAdd = new List<Run>();

        foreach (var record in totalRuns)
        {
            if (existingTetrioIds.Any(x => x == record.Id)) continue;

            var stats = record.Results.Stats;
            var clears = stats.Clears;

            var mods = record.Extras.Zenith.Mods;
            var totalSpins = clears.RealTspins
                             + clears.MiniTspins
                             + clears.MiniTspinSingles
                             + clears.TspinSingles
                             + clears.MiniTspindDoubles
                             + clears.TspinDoubles
                             + clears.MiniTspinTriples
                             + clears.TspinTriples
                             + clears.MiniTspinQuads
                             + clears.TspinQuads
                             + clears.TspinPentas;

            var splits = new ZenithSplit
            {
                User = user,
                TetrioId = record.Id,
                HotelReachedAt = (uint)stats.Zenith.Splits[0],
                CasinoReachedAt = (uint)stats.Zenith.Splits[1],
                ArenaReachedAt = (uint)stats.Zenith.Splits[2],
                MuseumReachedAt = (uint)stats.Zenith.Splits[3],
                OfficesReachedAt = (uint)stats.Zenith.Splits[4],
                LaboratoryReachedAt = (uint)stats.Zenith.Splits[5],
                CoreReachedAt = (uint)stats.Zenith.Splits[6],
                CorruptionReachedAt = (uint)stats.Zenith.Splits[7],
                PlatformOfTheGodsReachedAt = (uint)stats.Zenith.Splits[8]
            };

            var finesse = ((stats.Finesse!.Perfectpieces / stats.Piecesplaced ) * 100) ?? 0;

            var run = new Run
            {
                User = user,
                TetrioId = record.Id,
                PlayedAt = record.Ts,
                Altitude = stats.Zenith.Altitude ?? 0,
                KOs = (byte?)stats.Kills ?? 0,
                AllClears = (ushort?)clears.AllClear ?? 0,
                Quads = (ushort?)clears.Quads ?? 0,
                Spins = (ushort?)totalSpins ?? 0,
                Mods = string.Join(" ", mods),
                Apm = record.Results.Aggregatestats.Apm ?? 0,
                Pps = record.Results.Aggregatestats.Pps ?? 0,
                Vs = record.Results.Aggregatestats.Vsscore ?? 0,
                Finesse = finesse,
                SpeedrunSeen = stats.Zenith.SpeedrunSeen ?? false,
                SpeedrunCompleted = stats.Zenith.Speedrun ?? false,
            };

            var completedChallenges = new RunValidator().ValidateRun(challenges, run, mods);

            var completedChallengesSet = completedChallenges.ToHashSet();

            run.Challenges = completedChallengesSet;

            foreach (var challenge in completedChallengesSet)
            {
                user.Challenges.Add(challenge);
            }

            splitsToAdd.Add(splits);
            runsToAdd.Add(run);
        }

        user.LastSubmission = DateTime.UtcNow;

        await using var transaction = await _context.Database.BeginTransactionAsync();
        await _context.AddRangeAsync(splitsToAdd);
        await _context.AddRangeAsync(runsToAdd);
        var dataSavedToDb = await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        sw.Stop();

        Console.WriteLine($"[DAILY SUBMIT] {user.Username} saved {dataSavedToDb} columns successfully | Took {sw.Elapsed:g}");

        return Ok(new
        {
            Username = user.Username,
            EntriesSaved = dataSavedToDb,
            TimeToProcess = sw.Elapsed.ToString("g")
        });
    }

    [HttpGet]
    [Route("getLeaderboard")]
    public async Task<IActionResult> GetLeaderboard(int page = 1, int pageSize = 15)
    {
        var users = await _context.Users.Select(x => new
        {
            User = new
            {
                Name = x.Username
            },
            Challenges = x.Challenges.Count,
        }).OrderByDescending(x => x.Challenges).Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync();

        return Ok(users.Select(x => new
        {
            Username = x.User.Name,
            ChallengesCompleted = x.Challenges
        }));
    }

    [HttpGet]
    [Route("getSplitsLeaderboard")]
    public async Task<IActionResult> GetSplitLeaderboard()
    {
        return Ok();

        var minHotel = _context.ZenithSplits.Include(x => x.User).Min(x => x.HotelReachedAt);
        var minCasino = _context.ZenithSplits.Include(x => x.User).Min(x => x.CasinoReachedAt);
        var minArena = _context.ZenithSplits.Include(x => x.User).Min(x => x.ArenaReachedAt);
        var minMuseum = _context.ZenithSplits.Include(x => x.User).Min(x => x.MuseumReachedAt);
        var minOffices = _context.ZenithSplits.Include(x => x.User).Min(x => x.OfficesReachedAt);
        var minLaboratory = _context.ZenithSplits.Include(x => x.User).Min(x => x.LaboratoryReachedAt);
        var minCore = _context.ZenithSplits.Include(x => x.User).Min(x => x.CoreReachedAt);
        var minCorruption = _context.ZenithSplits.Include(x => x.User).Min(x => x.CorruptionReachedAt);
        var minPotg = _context.ZenithSplits.Include(x => x.User).Min(x => x.PlatformOfTheGodsReachedAt);


        return Ok(new
        {
            Hotel = new
            {
                Time = minHotel
            },
            Casino = new
            {
                Time = minCasino
            },
            Arena = new
            {
                Time = minArena
            },
            Museum = new
            {
                Time = minMuseum
            },
            Offices = new
            {
                Time = minOffices
            },
            Laboratory = new
            {
                Time = minLaboratory
            },
            Core = new
            {
                Time = minCore
            },
            Corruption = new
            {
                Time = minCorruption
            },
            Potg = new
            {
                Time = minPotg
            },
        });

        // return Ok(new
        // {
        //     Hotel = new
        //     {
        //         Time = minHotel?.HotelReachedAt,
        //         Username = minHotel?.User?.Username
        //     },
        //     Casino = new
        //     {
        //         Time = minCasino?.CasinoReachedAt,
        //         Username = minCasino?.User?.Username
        //     },
        //     Arena = new
        //     {
        //         Time = minArena?.ArenaReachedAt,
        //         Username = minArena?.User?.Username
        //     },
        //     Museum = new
        //     {
        //         Time = minMuseum?.MuseumReachedAt,
        //         Username = minMuseum?.User?.Username
        //     },
        //     Offices = new
        //     {
        //         Time = minOffices?.OfficesReachedAt,
        //         Username = minOffices?.User?.Username
        //     },
        //     Laboratory = new
        //     {
        //         Time = minLaboratory?.LaboratoryReachedAt,
        //         Username = minLaboratory?.User?.Username
        //     },
        //     Core = new
        //     {
        //         Time = minCore?.CoreReachedAt,
        //         Username = minCore?.User?.Username
        //     },
        //     Corruption = new
        //     {
        //         Time = minCorruption?.CorruptionReachedAt,
        //         Username = minCorruption?.User?.Username
        //     },
        //     Potg = new
        //     {
        //         Time = minPotg?.PlatformOfTheGodsReachedAt,
        //         Username = minPotg?.User?.Username
        //     },
        // });
    }
}