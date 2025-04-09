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
        var date = new DateTime(2020, 03, 22);
        var ranges = new List<ConditionRange>();

        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111101"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Easy, ConditionType = ConditionType.Height,    Min = 50,   Max = 350});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111102"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Easy, ConditionType = ConditionType.KOs,       Min = 0,    Max = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111103"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Easy, ConditionType = ConditionType.AllClears, Min = 0,    Max = 0});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111104"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Easy, ConditionType = ConditionType.Quads,     Min = 3,    Max = 10});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111105"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Easy, ConditionType = ConditionType.Spins,     Min = 0,    Max = 0});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111106"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Easy, ConditionType = ConditionType.Apm,       Min = 10,   Max = 20});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111107"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Easy, ConditionType = ConditionType.Pps,       Min = 0.75, Max = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111108"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Easy, ConditionType = ConditionType.Vs,        Min = 30,   Max = 40});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111109"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Easy, ConditionType = ConditionType.Finesse,   Min = 35,   Max = 50});

        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111201"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Normal, ConditionType = ConditionType.Height,    Min = 350, Max = 650});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111202"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Normal, ConditionType = ConditionType.KOs,       Min = 1,   Max = 2});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111203"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Normal, ConditionType = ConditionType.AllClears, Min = 0,   Max = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111204"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Normal, ConditionType = ConditionType.Quads,     Min = 5,   Max = 15});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111205"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Normal, ConditionType = ConditionType.Spins,     Min = 5,   Max = 30});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111206"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Normal, ConditionType = ConditionType.Apm,       Min = 20,  Max = 55});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111207"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Normal, ConditionType = ConditionType.Pps,       Min = 1,   Max = 1.75});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111208"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Normal, ConditionType = ConditionType.Vs,        Min = 40,  Max = 100});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111209"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Normal, ConditionType = ConditionType.Finesse,   Min = 50,  Max = 65});

        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111301"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Hard, ConditionType = ConditionType.Height,    Min = 650,  Max = 1350});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111302"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Hard, ConditionType = ConditionType.KOs,       Min = 2,    Max = 5});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111303"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Hard, ConditionType = ConditionType.AllClears, Min = 1,    Max = 3});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111304"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Hard, ConditionType = ConditionType.Quads,     Min = 10,   Max = 20});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111305"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Hard, ConditionType = ConditionType.Spins,     Min = 30,   Max = 75});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111306"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Hard, ConditionType = ConditionType.Apm,       Min = 55,   Max = 100});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111307"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Hard, ConditionType = ConditionType.Pps,       Min = 1.75, Max = 2.1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111308"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Hard, ConditionType = ConditionType.Vs,        Min = 100,  Max = 175});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111309"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Hard, ConditionType = ConditionType.Finesse,   Min = 65,   Max = 80});

        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111401"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Expert, ConditionType = ConditionType.Height,    Min = 1350, Max = 2500});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111402"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Expert, ConditionType = ConditionType.KOs,       Min = 3,    Max = 5});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111403"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Expert, ConditionType = ConditionType.AllClears, Min = 3,    Max = 6});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111404"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Expert, ConditionType = ConditionType.Quads,     Min = 20,   Max = 30});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111405"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Expert, ConditionType = ConditionType.Spins,     Min = 75,   Max = 125});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111406"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Expert, ConditionType = ConditionType.Apm,       Min = 85,   Max = 130});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111407"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Expert, ConditionType = ConditionType.Pps,       Min = 1.75, Max = 2.25});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111408"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Expert, ConditionType = ConditionType.Vs,        Min = 100,  Max = 200});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111409"), CreatedAt = date, UpdatedAt = date, Difficulty = Difficulty.Expert, ConditionType = ConditionType.Finesse,   Min = 75,   Max = 100});

        modelBuilder.Entity<ConditionRange>().HasData(ranges);
    }

    private static void GenerateMods(ModelBuilder modelBuilder)
    {
        var date = new DateTime(2020, 03, 22);
        var ranges = new List<Mod>();

        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111101"), CreatedAt = date, UpdatedAt = date, Name = "expert", MinDifficulty = Difficulty.Hard, Weight = 50});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111102"), CreatedAt = date, UpdatedAt = date, Name = "nohold", MinDifficulty = Difficulty.Easy, Weight = 25});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111103"), CreatedAt = date, UpdatedAt = date, Name = "messy", MinDifficulty = Difficulty.Easy, Weight = 25});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111104"), CreatedAt = date, UpdatedAt = date, Name = "gravity", MinDifficulty = Difficulty.Normal, Weight = 25});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111105"), CreatedAt = date, UpdatedAt = date, Name = "volatile", MinDifficulty = Difficulty.Normal, Weight = 25});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111106"), CreatedAt = date, UpdatedAt = date, Name = "doublehole", MinDifficulty = Difficulty.Normal, Weight = 25});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111107"), CreatedAt = date, UpdatedAt = date, Name = "invisible", MinDifficulty = Difficulty.Hard, Weight = 60});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111108"), CreatedAt = date, UpdatedAt = date, Name = "allspin", MinDifficulty = Difficulty.Normal, Weight = 40});

        modelBuilder.Entity<Mod>().HasData(ranges);
    }

}