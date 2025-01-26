using Microsoft.EntityFrameworkCore;
using Tetrio.Overlay.Database.Entities;
using Tetrio.Overlay.Database.Enums;

namespace Tetrio.Overlay.Database;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        GenerateConditionRanges(modelBuilder);

        GenerateMods(modelBuilder);
    }

    private static void GenerateConditionRanges(ModelBuilder modelBuilder)
    {
        var ranges = new List<ConditionRange>();

        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Easy, ConditionType = ConditionType.Height,    Min = 50, Max = 350});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Easy, ConditionType = ConditionType.KOs,       Min = 0,   Max = 1});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Easy, ConditionType = ConditionType.AllClears, Min = 0,   Max = 0});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Easy, ConditionType = ConditionType.Quads,     Min = 3,   Max = 10});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Easy, ConditionType = ConditionType.Spins,     Min = 0,   Max = 0});

        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Normal, ConditionType = ConditionType.Height,    Min = 350, Max = 650});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Normal, ConditionType = ConditionType.KOs,       Min = 1,   Max = 2});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Normal, ConditionType = ConditionType.AllClears, Min = 0,   Max = 1});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Normal, ConditionType = ConditionType.Quads,     Min = 5,   Max = 15});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Normal, ConditionType = ConditionType.Spins,     Min = 5,   Max = 30});

        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Hard, ConditionType = ConditionType.Height,    Min = 650, Max = 1350});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Hard, ConditionType = ConditionType.KOs,       Min = 2,   Max = 5});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Hard, ConditionType = ConditionType.AllClears, Min = 1,   Max = 3});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Hard, ConditionType = ConditionType.Quads,     Min = 10,   Max = 20});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Hard, ConditionType = ConditionType.Spins,     Min = 30,   Max = 75});

        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Expert, ConditionType = ConditionType.Height,    Min = 1350, Max = 2500});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Expert, ConditionType = ConditionType.KOs,       Min = 3,   Max = 5});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Expert, ConditionType = ConditionType.AllClears, Min = 3,   Max = 6});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Expert, ConditionType = ConditionType.Quads,     Min = 20,   Max = 30});
        ranges.Add(new () {Id = Guid.NewGuid(), Difficulty = Difficulty.Expert, ConditionType = ConditionType.Spins,     Min = 75,   Max = 125});

        modelBuilder.Entity<ConditionRange>().HasData(ranges);
    }

    private static void GenerateMods(ModelBuilder modelBuilder)
    {
        var ranges = new List<Mod>();

        ranges.Add(new () {Id = Guid.NewGuid(), Name = "expert", MinDifficulty = Difficulty.Hard, Weight = 50});
        ranges.Add(new () {Id = Guid.NewGuid(), Name = "nohold", MinDifficulty = Difficulty.Easy, Weight = 25});
        ranges.Add(new () {Id = Guid.NewGuid(), Name = "messy", MinDifficulty = Difficulty.Easy, Weight = 25});
        ranges.Add(new () {Id = Guid.NewGuid(), Name = "gravity", MinDifficulty = Difficulty.Normal, Weight = 25});
        ranges.Add(new () {Id = Guid.NewGuid(), Name = "volatile", MinDifficulty = Difficulty.Normal, Weight = 25});
        ranges.Add(new () {Id = Guid.NewGuid(), Name = "doublehole", MinDifficulty = Difficulty.Normal, Weight = 25});
        ranges.Add(new () {Id = Guid.NewGuid(), Name = "invisible", MinDifficulty = Difficulty.Hard, Weight = 60});
        ranges.Add(new () {Id = Guid.NewGuid(), Name = "allspin", MinDifficulty = Difficulty.Normal, Weight = 40});

        modelBuilder.Entity<Mod>().HasData(ranges);
    }

}