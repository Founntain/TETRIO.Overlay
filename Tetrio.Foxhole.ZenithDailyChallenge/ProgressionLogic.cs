using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Entities;
using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Zenith.DailyChallenge;

public class ProgressionLogic
{
    private readonly TetrioContext _context;
    private User _user;

    private List<Progression> _cachedPbs = new List<Progression>();

    public ProgressionLogic(TetrioContext context, User user)
    {
        _context = context;
        _user = user;
    }

    public async Task InitializeCache()
    {
        _cachedPbs = await _context.Progressions.AsNoTracking().Where(x => x.User.Id == _user.Id && x.IsPersonalBest).ToListAsync();
    }

    private readonly record struct FloorConfig(ZenithFloor Floor, uint Value);

    public async Task ProcessAltitudeProgression(Run run)
    {
        if (string.IsNullOrWhiteSpace(run.Mods))
        {
            var topProgression = _cachedPbs.Where(x => x.Type == ProgressionType.Altitude).MaxBy(x => x.Value);

            if (topProgression?.Value <= run.Altitude)
            {
                var progression = new Progression()
                {
                    UserId = _user.Id,
                    Value = run.Altitude,
                    Type = ProgressionType.Altitude,
                    Mods = null,
                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                    IsPersonalBest = true
                };

                await _context.AddAsync(progression);
                _cachedPbs.Add(progression);
            }
        }
        else
        {
            var mods = run.Mods.Split(' ');

            foreach (var mod in mods)
            {
                if (mod.Contains("snowman") || mod.Contains("pento")) continue;

                var isModPb = _cachedPbs.Where(x => x.Type == ProgressionType.Altitude && !string.IsNullOrWhiteSpace(x.Mods) && x.Mods.Contains(mod)).MaxBy(x => x.Value);

                if (!(isModPb?.Value <= run.Altitude)) continue;

                var modProgression = new Progression()
                {
                    UserId = _user.Id,
                    TetrioId = run.TetrioId,
                    Value = run.Altitude,
                    Type = ProgressionType.Altitude,
                    Mods = string.IsNullOrWhiteSpace(run.Mods) ? null : run.Mods,
                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                    IsPersonalBest = true,
                };

                await _context.AddAsync(modProgression);
                _cachedPbs.Add(modProgression);
            }
        }
    }

    public async Task ProcessSplitProgression(Run run, ZenithSplit? splits)
    {
        if (splits == null) return;

        var runSplits = await _context.ZenithSplits.AsNoTracking().FirstOrDefaultAsync(x => x.User.Id == _user.Id && x.TetrioId == run.TetrioId);

        if (runSplits == null) return;

        var floorConfigs = new[]
        {
            new FloorConfig(ZenithFloor.Hotel,runSplits.HotelReachedAt),
            new FloorConfig(ZenithFloor.Casino,runSplits.CasinoReachedAt),
            new FloorConfig(ZenithFloor.Arena,runSplits.ArenaReachedAt),
            new FloorConfig(ZenithFloor.Museum,runSplits.MuseumReachedAt),
            new FloorConfig(ZenithFloor.Offices,runSplits.OfficesReachedAt),
            new FloorConfig(ZenithFloor.Laboratory,runSplits.LaboratoryReachedAt),
            new FloorConfig(ZenithFloor.Core,runSplits.CoreReachedAt),
            new FloorConfig(ZenithFloor.Corruption,runSplits.CorruptionReachedAt),
            new FloorConfig(ZenithFloor.PlatformOfTheGods,runSplits.PlatformOfTheGodsReachedAt)
        };

        if (string.IsNullOrWhiteSpace(run.Mods))
        {
            await ProcessNoModSplits(run, floorConfigs);
        }
        else
        {
            await ProcessModdedSplits(run, floorConfigs);
        }
    }

    private async Task ProcessNoModSplits(Run run, ICollection<FloorConfig> floorConfigs)
    {
        foreach (var config in floorConfigs)
        {
            if (config.Value <= 0) continue;

            var isPb = !_cachedPbs.Where(x => x.Type == ProgressionType.ZenithSplit && string.IsNullOrWhiteSpace(x.Mods)).Any(x => x.Floor == config.Floor && x.Value < config.Value);

            if (!isPb) continue;

            var progression = new Progression
            {
                UserId = _user.Id,
                Type = ProgressionType.ZenithSplit,
                Value = config.Value,
                PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                IsPersonalBest = true,
                Floor = config.Floor,
            };

            await _context.AddAsync(progression);
            _cachedPbs.Add(progression);
        }
    }

    private async Task ProcessModdedSplits(Run run, ICollection<FloorConfig> floorConfigs)
    {
        var mods = run.Mods.Split(' ');

        foreach (var mod in mods)
        {
            if (mod.Contains("snowman") || mod.Contains("pento")) continue;

            foreach (var config in floorConfigs)
            {
                if (config.Value <= 0) continue;

                var isPb = !_cachedPbs.Where(x => x.Type == ProgressionType.ZenithSplit && !string.IsNullOrWhiteSpace(x.Mods) && !(x.Mods.Contains("snowman") || x.Mods.Contains("pento"))).Any(x => x.Floor == config.Floor && x.Mods!.Contains(mod) && x.Value < config.Value);

                if (!isPb) continue;

                var progression = new Progression
                {
                    UserId = _user.Id,
                    Type = ProgressionType.ZenithSplit,
                    Value = config.Value,
                    PlayedAt = run.PlayedAt ?? DateTime.UtcNow,
                    IsPersonalBest = true,
                    Floor = config.Floor,
                    Mods = run.Mods
                };

                await _context.AddAsync(progression);
                _cachedPbs.Add(progression);
            }
        }
    }
}