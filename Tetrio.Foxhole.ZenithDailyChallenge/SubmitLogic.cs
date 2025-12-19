using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Entities;
using Tetrio.Foxhole.Database.Enums;
using Tetrio.Foxhole.Network.Api.Tetrio;
using Tetrio.Foxhole.Network.Api.Tetrio.Models;

namespace Tetrio.Zenith.DailyChallenge;

public class SubmitLogic
{
    private readonly TetrioContext _context;
    private User _user;
    private DateOnly _day;
    private TetrioApi Api { get; set; }

    public SubmitLogic(TetrioContext context, TetrioApi api, User user, DateOnly day)
    {
        _context = context;
        _user = user;
        _day = day;
        Api = api;
    }

    public async Task<(int ResponseCode, object ResultObject)> ProcessSubmissions()
    {
        var now = DateTime.UtcNow;
        var nextSubmissionPossible = _user.LastSubmission?.AddMinutes(1) ?? DateTime.MinValue;

        if(nextSubmissionPossible > DateTime.UtcNow) return (400, "Please wait 1 minute before requesting submitting daily challenges again.");

        _day = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var challenges = await _context.Challenges.Where(x => x.Date == _day).OrderByDescending(x => x.Points).ToListAsync();

        if (challenges.Count == 0) return (200, "No daily challenges found, submission canceled");

        var summary = await Api.GetTetraLeagueStats(_user.Username);
        var records = await Api.GetRecentZenithRecords(_user.Username);
        var expertRecords = await Api.GetRecentZenithRecords(_user.Username, true);

        if (summary != null)
        {
            _user.TetrioRank = summary?.Rank;

            await _context.SaveChangesAsync();
        }

        if (records == null || expertRecords == null) return (200, "Could not fetch your recent records, please try again later");

        var totalRuns = new List<Record>();

        totalRuns.AddRange(records.Entries);
        totalRuns.AddRange(expertRecords.Entries);

        totalRuns = totalRuns.DistinctBy(x => x.Id).ToList();

        var communityChallenge = await _context.CommunityChallenges.FirstOrDefaultAsync(x => x.StartDate <= now && x.EndDate >= now);

        Console.WriteLine($"[DAILY SUBMIT] {_user.Username} submitted {totalRuns.Count} run(s)]");

        var tetrioIds = totalRuns.Select(x => x.Id).ToList();

        var existingTetrioIds = new HashSet<string>(await _context.Runs.Where(x => tetrioIds.Contains(x.TetrioId)).Select(x => x.TetrioId).ToListAsync());

        var sw = new Stopwatch();
        sw.Restart();

        var splitsToAdd = new List<ZenithSplit>();
        var runsToAdd = new List<Run>();
        var everyClear = new List<Clears>();
        var runValidator = new RunValidator();

        foreach (var run in totalRuns)
        {
            if (existingTetrioIds.Any(x => x == run.Id)) continue;
            if (string.IsNullOrWhiteSpace(run.Id)) continue;

            var stats = run.Results.Stats;

            var processResult = await ProcessRun(challenges, runValidator, run);

            // Run processing was aborted or canceled and therefore skipped
            if(processResult.Run == null) continue;

            var clears = stats.Clears;
            everyClear.Add(clears);

            if(processResult.Splits != null)
                splitsToAdd.Add(processResult.Splits);

            runsToAdd.Add(processResult.Run);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        _user.LastSubmission = DateTime.UtcNow;

        if (communityChallenge != null)
        {
            var contribution = ProcessCommunityContribution(runValidator, communityChallenge, runsToAdd, everyClear);

            if (contribution != null)
                await _context.AddAsync(contribution);
        }

        var todaysRuns = runsToAdd.Where(x => _day == DateOnly.FromDateTime(x.PlayedAt!.Value.Date)).ToList();

        if (todaysRuns.Any())
        {
            var masteryAttempt= await runValidator.ValidateMasteryChallenge(_user, _day, _context, todaysRuns);

            if (masteryAttempt != null && !await _context.MasteryAttempts.AnyAsync(x => x.MasteryChallengeId == masteryAttempt.MasteryChallengeId && x.UserId == _user.Id))
            {
                await _context.AddAsync(masteryAttempt);
            }
        }

        await _context.AddRangeAsync(splitsToAdd);
        await _context.AddRangeAsync(runsToAdd);
        var dataSavedToDb = await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        sw.Stop();

        Console.WriteLine($"[DAILY SUBMIT] {_user.Username} saved {dataSavedToDb} columns successfully | Took {sw.Elapsed:g}");

        return (200, new
        {
            Username = _user.Username,
            EntriesSaved = dataSavedToDb,
            TimeToProcess = sw.Elapsed.ToString("g")
        });
    }

    private async Task<(Run? Run, ZenithSplit? Splits)> ProcessRun(List<Challenge> challenges, RunValidator runValidator, Record record)
    {
        var stats = record.Results.Stats;
        var clears = stats.Clears;

        // If the altitude is lower than 10M we ignore these runs, they dont have any meaning full data
        if ((stats.Zenith.Altitude ?? 0) < 10)
        {
            return (null, null);
        }

        var mods = record.Extras.Zenith.Mods;
        var totalSpins = clears.RealTspins // Zero Spins
                         + clears.MiniTspins // Zero Mini Spins
                         + clears.MiniTspinSingles
                         + clears.TspinSingles
                         + clears.MiniTspinDoubles
                         + clears.TspinDoubles
                         + clears.MiniTspinTriples
                         + clears.TspinTriples
                         + clears.MiniTspinQuads
                         + clears.TspinQuads
                         + clears.TspinPentas;

        var hotelSplit = stats.Zenith.Splits[0] ?? 0;

        ZenithSplit? splits = null;

        // We only want to create splits when we at least reached the hotel, otherwise we just create empty rows
        if (hotelSplit > 0)
        {
            splits = new ZenithSplit
            {
                User = _user,
                TetrioId = record.Id,
                DatePlayed = record.Ts ?? DateTime.MinValue,
                HotelReachedAt = (uint)(stats.Zenith.Splits[0] ?? 0),
                CasinoReachedAt = (uint)(stats.Zenith.Splits[1] ?? 0),
                ArenaReachedAt = (uint)(stats.Zenith.Splits[2] ?? 0),
                MuseumReachedAt = (uint)(stats.Zenith.Splits[3] ?? 0),
                OfficesReachedAt = (uint)(stats.Zenith.Splits[4] ?? 0),
                LaboratoryReachedAt = (uint)(stats.Zenith.Splits[5] ?? 0),
                CoreReachedAt = (uint)(stats.Zenith.Splits[6] ?? 0),
                CorruptionReachedAt = (uint)(stats.Zenith.Splits[7] ?? 0),
                PlatformOfTheGodsReachedAt = (uint)(stats.Zenith.Splits[8] ?? 0),

                Mods = mods.Length == 0 ? null : string.Join(" ", mods)
            };
        }

        var finesse = 0d;

        if(stats.Piecesplaced > 0 && stats.Finesse!.Perfectpieces > 0)
        {
            finesse = ((stats.Finesse!.Perfectpieces / stats.Piecesplaced ) * 100).Value;
        }

        var run = Run.Create(_user, record, stats, clears, finesse, totalSpins, mods);

        if (run.PlayedAt?.Date == null)
        {
            Console.WriteLine($"No date found for run {run.TetrioId} | Skipping run");

            return (null, null);
        }

        var playedAtDay = DateOnly.FromDateTime(run.PlayedAt!.Value.Date);

        if (_day == playedAtDay)
        {
            var completedChallenges = runValidator.ValidateRun(challenges, run, mods);
            var completedChallengesSet = completedChallenges.ToHashSet();

            run.Challenges = completedChallengesSet;

            foreach (var challenge in completedChallengesSet)
            {
                // Only add the challenge if it wasn't already completed by the user, if not ignore it
                if (_user.Challenges.FirstOrDefault(x => x.Id == challenge.Id) != null) continue;

                _user.Challenges.Add(challenge);

                var scoreToAdd = CalculateScoreFromChallenge(challenge);
                _user.Score += scoreToAdd;
            }
        }
        else
        {
            Console.WriteLine($"Run {run.TetrioId} was played on a different day, added but its not counted for daily challenge");
        }

        return (run, splits);
    }

    private uint CalculateScoreFromChallenge(Challenge challenge)
    {
        switch ((Difficulty) challenge.Points)
        {
            case Difficulty.Easy:
            case Difficulty.Normal:
            case Difficulty.Hard:
            case Difficulty.Expert:
                return challenge.Points;
            case Difficulty.Reverse:
                return (uint) (challenge.Points / 2);
                break;
            default: return 0;
        }
    }

    private CommunityContribution? ProcessCommunityContribution(RunValidator runValidator, CommunityChallenge communityChallenge, List<Run> runs, List<Clears> clears)
    {
        var contribution = new CommunityContribution()
        {
            CommunityChallenge = communityChallenge,
        };

        var validRuns = runs.Where(x => x.TotalTime > 60000 && x.PlayedAt >= communityChallenge.StartDate).ToList();

        if(validRuns.Count == 0) return null;

        // Check if all required mods are used if they exist
        if (communityChallenge.Mods?.Length > 0)
        {
            var requiredMods = string.IsNullOrWhiteSpace(communityChallenge.Mods) ? [] : communityChallenge.Mods.Split(" ");

            if(communityChallenge.RequireAllMods)
                validRuns = validRuns.Where(x => !string.IsNullOrWhiteSpace(x.Mods) && x.Mods.Length > 0 && requiredMods.All(mod => x.Mods.Contains(mod))).ToList();
            else
                validRuns = validRuns.Where(x => !string.IsNullOrWhiteSpace(x.Mods) && x.Mods.Length > 0 && requiredMods.Any(mod => x.Mods.Contains(mod))).ToList();
        }

        if(validRuns.Count == 0) return null;

        runValidator.UpdateAmountAccordingToRuns(ref contribution, communityChallenge.ConditionType, validRuns, clears);

        if (contribution.Amount <= 0) return null;

        contribution.User = _user;

        // Add IsLate flag when it's finished, so it shows up in the feat, but not on the leaderboard
        if (communityChallenge.Finished) contribution.IsLate = true;

        // Increase amount of a community challenge, as long as it is active.
        // Doesn't matter if finished OR NOT
        // If Value is equal or bigger than TargetValue, set finished to true
        communityChallenge.Value += contribution.Amount;
        communityChallenge.Finished = communityChallenge.Value >= communityChallenge.TargetValue;
        // If the community challenge is finished, we also want to show the mods to the user in case they did not solved it so far.
        if(communityChallenge.ShowMods == false) communityChallenge.ShowMods = communityChallenge.Finished;

        Console.WriteLine($"[CC] Added {contribution.Amount} from {_user.Username}. With runs: {string.Join(' ', runs.Select(x => x.TetrioId))}");

        return contribution;
    }
}