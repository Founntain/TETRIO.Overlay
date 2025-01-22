using System.Text.Json.Serialization;

namespace TetraLeague.Overlay.Network.Api.Models;

public class ApiRecord
{
    [JsonPropertyName("record")] public Record? Record { get; set; }

    [JsonPropertyName("rank")] public double? Rank { get; set; }

    [JsonPropertyName("rank_local")] public double? RankLocal { get; set; }

    [JsonPropertyName("best")] public ApiRecord? Best { get; set; }
}

public class Aggregatestats
{
    [JsonPropertyName("apm")] public double? Apm { get; set; }

    [JsonPropertyName("pps")] public double? Pps { get; set; }

    [JsonPropertyName("vsscore")] public double? Vsscore { get; set; }
}

public class Clears
{
    [JsonPropertyName("singles")] public double? Singles { get; set; }

    [JsonPropertyName("doubles")] public double? Doubles { get; set; }

    [JsonPropertyName("triples")] public double? Triples { get; set; }

    [JsonPropertyName("quads")] public double? Quads { get; set; }

    [JsonPropertyName("pentas")] public double? Pentas { get; set; }

    [JsonPropertyName("realtspins")] public double? Realtspins { get; set; }

    [JsonPropertyName("minitspins")] public double? Minitspins { get; set; }

    [JsonPropertyName("minitspinsingles")] public double? Minitspinsingles { get; set; }

    [JsonPropertyName("tspinsingles")] public double? Tspinsingles { get; set; }

    [JsonPropertyName("minitspindoubles")] public double? Minitspindoubles { get; set; }

    [JsonPropertyName("tspindoubles")] public double? Tspindoubles { get; set; }

    [JsonPropertyName("minitspdoubleriples")] public double? Minitspdoubleriples { get; set; }

    [JsonPropertyName("tspdoubleriples")] public double? Tspdoubleriples { get; set; }

    [JsonPropertyName("minitspinquads")] public double? Minitspinquads { get; set; }

    [JsonPropertyName("tspinquads")] public double? Tspinquads { get; set; }

    [JsonPropertyName("tspinpentas")] public double? Tspinpentas { get; set; }

    [JsonPropertyName("allclear")] public double? Allclear { get; set; }
}

public class Extras
{
    public object League { get; set; }
    public object Result { get; set; }
    public ZenithExtra Zenith { get; set; }
}

public class ZenithExtra
{
    [JsonPropertyName("mods")] public string[] Mods { get; set; }
}

public class Finesse
{
    [JsonPropertyName("combo")] public double? Combo { get; set; }

    [JsonPropertyName("faults")] public double? Faults { get; set; }

    [JsonPropertyName("perfectpieces")] public double? Perfectpieces { get; set; }
}

public class Garbage
{
    [JsonPropertyName("sent")] public double? Sent { get; set; }

    [JsonPropertyName("sent_nomult")] public double? SentNomult { get; set; }

    [JsonPropertyName("maxspike")] public double? Maxspike { get; set; }

    [JsonPropertyName("maxspike_nomult")] public double? MaxspikeNomult { get; set; }

    [JsonPropertyName("received")] public double? Received { get; set; }

    [JsonPropertyName("attack")] public double? Attack { get; set; }

    [JsonPropertyName("cleared")] public double? Cleared { get; set; }
}

public class P
{
    [JsonPropertyName("pri")] public double? Pri { get; set; }

    [JsonPropertyName("sec")] public double? Sec { get; set; }

    [JsonPropertyName("ter")] public double? Ter { get; set; }
}

public class Record
{
    [JsonPropertyName("_id")] public string Id { get; set; }

    [JsonPropertyName("replayid")] public string Replayid { get; set; }

    [JsonPropertyName("stub")] public bool? Stub { get; set; }

    [JsonPropertyName("gamemode")] public string Gamemode { get; set; }

    [JsonPropertyName("pb")] public bool? Pb { get; set; }

    [JsonPropertyName("oncepb")] public bool? Oncepb { get; set; }

    [JsonPropertyName("ts")] public DateTime? Ts { get; set; }

    [JsonPropertyName("revolution")] public object Revolution { get; set; }

    [JsonPropertyName("user")] public User User { get; set; }

    [JsonPropertyName("otherusers")] public List<object> Otherusers { get; set; }

    [JsonPropertyName("leaderboards")] public List<string> Leaderboards { get; set; }

    [JsonPropertyName("results")] public Results Results { get; set; }

    [JsonPropertyName("extras")] public Extras Extras { get; set; }

    [JsonPropertyName("disputed")] public bool? Disputed { get; set; }

    [JsonPropertyName("p")] public P P { get; set; }
}

public class Results
{
    [JsonPropertyName("aggregatestats")] public Aggregatestats Aggregatestats { get; set; }

    [JsonPropertyName("stats")] public Stats Stats { get; set; }

    [JsonPropertyName("gameoverreason")] public string Gameoverreason { get; set; }
}

public class Stats
{
    [JsonPropertyName("lines")] public double? Lines { get; set; }

    [JsonPropertyName("level_lines")] public double? LevelLines { get; set; }

    [JsonPropertyName("level_lines_needed")]
    public double? LevelLinesNeeded { get; set; }

    [JsonPropertyName("inputs")] public double? Inputs { get; set; }

    [JsonPropertyName("holds")] public double? Holds { get; set; }

    [JsonPropertyName("score")] public double? Score { get; set; }

    [JsonPropertyName("zenlevel")] public double? Zenlevel { get; set; }

    [JsonPropertyName("zenprogress")] public double? Zenprogress { get; set; }

    [JsonPropertyName("level")] public double? Level { get; set; }

    [JsonPropertyName("combo")] public double? Combo { get; set; }

    [JsonPropertyName("topcombo")] public double? Topcombo { get; set; }

    [JsonPropertyName("combopower")] public double? Combopower { get; set; }

    [JsonPropertyName("btb")] public double? Btb { get; set; }

    [JsonPropertyName("topbtb")] public double? Topbtb { get; set; }

    [JsonPropertyName("btbpower")] public double? Btbpower { get; set; }

    [JsonPropertyName("tspins")] public double? Tspins { get; set; }

    [JsonPropertyName("piecesplaced")] public double? Piecesplaced { get; set; }

    [JsonPropertyName("clears")] public Clears Clears { get; set; }

    [JsonPropertyName("garbage")] public Garbage Garbage { get; set; }

    [JsonPropertyName("kills")] public double? Kills { get; set; }

    [JsonPropertyName("finesse")] public Finesse? Finesse { get; set; }

    [JsonPropertyName("zenith")] public Zenith Zenith { get; set; }

    [JsonPropertyName("finaltime")] public double? Finaltime { get; set; }
}

public class User
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("username")] public string Username { get; set; }

    [JsonPropertyName("avatar_revision")] public long? AvatarRevision { get; set; }

    [JsonPropertyName("banner_revision")] public long? BannerRevision { get; set; }

    [JsonPropertyName("country")] public string Country { get; set; }

    [JsonPropertyName("supporter")] public bool? Supporter { get; set; }
}

public class Zenith
{
    [JsonPropertyName("altitude")] public double? Altitude { get; set; }

    [JsonPropertyName("rank")] public double? Rank { get; set; }

    [JsonPropertyName("peakrank")] public double? Peakrank { get; set; }

    [JsonPropertyName("avgrankpts")] public double? Avgrankpts { get; set; }

    [JsonPropertyName("floor")] public double? Floor { get; set; }

    [JsonPropertyName("targetingfactor")] public double? Targetingfactor { get; set; }

    [JsonPropertyName("targetinggrace")] public double? Targetinggrace { get; set; }

    [JsonPropertyName("totalbonus")] public double? Totalbonus { get; set; }

    [JsonPropertyName("revives")] public double? Revives { get; set; }

    [JsonPropertyName("revivesTotal")] public double? RevivesTotal { get; set; }

    [JsonPropertyName("speedrun")] public bool? Speedrun { get; set; }

    [JsonPropertyName("speedrun_seen")] public bool? SpeedrunSeen { get; set; }

    [JsonPropertyName("splits")] public List<double?> Splits { get; set; }
}