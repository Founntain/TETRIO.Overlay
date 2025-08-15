using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Database.Entities;
using Tetrio.Foxhole.Database.Enums;

namespace Tetrio.Foxhole.Database;

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

        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111001"), Difficulty = Difficulty.Community, ConditionType = ConditionType.Height,     Min = 1000000, Max = 5000000});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111002"), Difficulty = Difficulty.Community, ConditionType = ConditionType.KOs,        Min = 10000,   Max = 15000});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111003"), Difficulty = Difficulty.Community, ConditionType = ConditionType.AllClears,  Min = 50000,   Max = 100000});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111004"), Difficulty = Difficulty.Community, ConditionType = ConditionType.Quads,      Min = 45000,   Max = 100000});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111005"), Difficulty = Difficulty.Community, ConditionType = ConditionType.Spins,      Min = 250000,  Max = 1000000});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111006"), Difficulty = Difficulty.Community, ConditionType = ConditionType.Apm,        Min = 100000,  Max = 7500000});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111007"), Difficulty = Difficulty.Community, ConditionType = ConditionType.Pps,        Min = 5000,    Max = 15000});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111008"), Difficulty = Difficulty.Community, ConditionType = ConditionType.Vs,         Min = 750000,  Max = 1500000});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111009"), Difficulty = Difficulty.Community, ConditionType = ConditionType.Finesse,    Min = 0,       Max = 0});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111010"), Difficulty = Difficulty.Community, ConditionType = ConditionType.BackToBack, Min = 250000,  Max = 1000000});

        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111101"), Difficulty = Difficulty.Easy, ConditionType = ConditionType.Height,     Min = 100,  Max = 350});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111102"), Difficulty = Difficulty.Easy, ConditionType = ConditionType.KOs,        Min = 0,    Max = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111103"), Difficulty = Difficulty.Easy, ConditionType = ConditionType.AllClears,  Min = 0,    Max = 0});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111104"), Difficulty = Difficulty.Easy, ConditionType = ConditionType.Quads,      Min = 3,    Max = 10});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111105"), Difficulty = Difficulty.Easy, ConditionType = ConditionType.Spins,      Min = 0,    Max = 0});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111106"), Difficulty = Difficulty.Easy, ConditionType = ConditionType.Apm,        Min = 10,   Max = 20});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111107"), Difficulty = Difficulty.Easy, ConditionType = ConditionType.Pps,        Min = 0.75, Max = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111108"), Difficulty = Difficulty.Easy, ConditionType = ConditionType.Vs,         Min = 30,   Max = 40});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111109"), Difficulty = Difficulty.Easy, ConditionType = ConditionType.Finesse,    Min = 35,   Max = 50});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111110"), Difficulty = Difficulty.Easy, ConditionType = ConditionType.BackToBack, Min = 3,    Max = 7});

        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111201"), Difficulty = Difficulty.Normal, ConditionType = ConditionType.Height,     Min = 350, Max = 650});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111202"), Difficulty = Difficulty.Normal, ConditionType = ConditionType.KOs,        Min = 1,   Max = 2});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111203"), Difficulty = Difficulty.Normal, ConditionType = ConditionType.AllClears,  Min = 0,   Max = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111204"), Difficulty = Difficulty.Normal, ConditionType = ConditionType.Quads,      Min = 5,   Max = 15});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111205"), Difficulty = Difficulty.Normal, ConditionType = ConditionType.Spins,      Min = 5,   Max = 30});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111206"), Difficulty = Difficulty.Normal, ConditionType = ConditionType.Apm,        Min = 20,  Max = 55});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111207"), Difficulty = Difficulty.Normal, ConditionType = ConditionType.Pps,        Min = 1,   Max = 1.65});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111208"), Difficulty = Difficulty.Normal, ConditionType = ConditionType.Vs,         Min = 40,  Max = 95});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111209"), Difficulty = Difficulty.Normal, ConditionType = ConditionType.Finesse,    Min = 50,  Max = 65});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111210"), Difficulty = Difficulty.Normal, ConditionType = ConditionType.BackToBack, Min = 7,   Max = 25});

        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111301"), Difficulty = Difficulty.Hard, ConditionType = ConditionType.Height,     Min = 650,  Max = 1350});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111302"), Difficulty = Difficulty.Hard, ConditionType = ConditionType.KOs,        Min = 2,    Max = 5});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111303"), Difficulty = Difficulty.Hard, ConditionType = ConditionType.AllClears,  Min = 1,    Max = 3});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111304"), Difficulty = Difficulty.Hard, ConditionType = ConditionType.Quads,      Min = 15,   Max = 30});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111305"), Difficulty = Difficulty.Hard, ConditionType = ConditionType.Spins,      Min = 30,   Max = 75});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111306"), Difficulty = Difficulty.Hard, ConditionType = ConditionType.Apm,        Min = 55,   Max = 100});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111307"), Difficulty = Difficulty.Hard, ConditionType = ConditionType.Pps,        Min = 1.65, Max = 2.1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111308"), Difficulty = Difficulty.Hard, ConditionType = ConditionType.Vs,         Min = 80,   Max = 175});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111309"), Difficulty = Difficulty.Hard, ConditionType = ConditionType.Finesse,    Min = 65,   Max = 80});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111310"), Difficulty = Difficulty.Hard, ConditionType = ConditionType.BackToBack, Min = 25,   Max = 50});

        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111401"), Difficulty = Difficulty.Expert, ConditionType = ConditionType.Height,     Min = 650,  Max = 1100});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111402"), Difficulty = Difficulty.Expert, ConditionType = ConditionType.KOs,        Min = 2,    Max = 5});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111403"), Difficulty = Difficulty.Expert, ConditionType = ConditionType.AllClears,  Min = 0,    Max = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111404"), Difficulty = Difficulty.Expert, ConditionType = ConditionType.Quads,      Min = 20,   Max = 30});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111405"), Difficulty = Difficulty.Expert, ConditionType = ConditionType.Spins,      Min = 50,   Max = 100});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111406"), Difficulty = Difficulty.Expert, ConditionType = ConditionType.Apm,        Min = 50,   Max = 80});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111407"), Difficulty = Difficulty.Expert, ConditionType = ConditionType.Pps,        Min = 1.4,  Max = 1.75});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111408"), Difficulty = Difficulty.Expert, ConditionType = ConditionType.Vs,         Min = 80,   Max = 125});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111409"), Difficulty = Difficulty.Expert, ConditionType = ConditionType.Finesse,    Min = 60,   Max = 80});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111410"), Difficulty = Difficulty.Expert, ConditionType = ConditionType.BackToBack, Min = 20,   Max = 80});

        modelBuilder.Entity<ConditionRange>().HasData(ranges);
    }

    private static void GenerateMods(ModelBuilder modelBuilder)
    {
        var ranges = new List<Mod>();

        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111101"), Name = "expert",     MinDifficulty = Difficulty.Hard,   Weight = 60, Scaling = 0.75});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111102"), Name = "nohold",     MinDifficulty = Difficulty.Easy,   Weight = 25, Scaling = 0.90});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111103"), Name = "messy",      MinDifficulty = Difficulty.Easy,   Weight = 25, Scaling = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111104"), Name = "gravity",    MinDifficulty = Difficulty.Normal, Weight = 30, Scaling = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111105"), Name = "volatile",   MinDifficulty = Difficulty.Normal, Weight = 25, Scaling = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111106"), Name = "doublehole", MinDifficulty = Difficulty.Normal, Weight = 25, Scaling = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111107"), Name = "invisible",  MinDifficulty = Difficulty.Hard,   Weight = 60, Scaling = 1});
        ranges.Add(new () {Id = new Guid("11111111-1111-1111-1111-111111111108"), Name = "allspin",    MinDifficulty = Difficulty.Normal, Weight = 40, Scaling = 1});

        modelBuilder.Entity<Mod>().HasData(ranges);
    }

}