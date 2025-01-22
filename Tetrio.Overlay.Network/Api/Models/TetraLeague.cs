using System.Text.Json.Serialization;

namespace TetraLeague.Overlay.Network.Api.Models;

public class TetraLeague
{
    [JsonPropertyName("gamesplayed")]
    public int? Gamesplayed { get; set; }

    [JsonPropertyName("gameswon")]
    public int? Gameswon { get; set; }

    [JsonPropertyName("glicko")]
    public double? Glicko { get; set; }

    [JsonPropertyName("rd")]
    public double? Rd { get; set; }

    [JsonPropertyName("tr")]
    public double? Tr { get; set; }

    [JsonPropertyName("gxe")]
    public double? Gxe { get; set; }

    [JsonPropertyName("rank")]
    public string? Rank { get; set; }

    [JsonPropertyName("bestrank")]
    public string? TopRank { get; set; }

    [JsonPropertyName("apm")]
    public double? Apm { get; set; }

    [JsonPropertyName("pps")]
    public double? Pps { get; set; }

    [JsonPropertyName("vs")]
    public double? Vs { get; set; }

    [JsonPropertyName("decaying")]
    public bool? Decaying { get; set; }

    [JsonPropertyName("past")]
    public dynamic? Past { get; set; }

    [JsonPropertyName("standing_local")]
    public int? StandingLocal { get; set; }

    [JsonPropertyName("standing")]
    public int? StandingGlobal { get; set; }

    [JsonPropertyName("prev_rank")]
    public string? PrevRank { get; set; }

    [JsonPropertyName("prev_at")]
    public int? PrevAt { get; set; }

    [JsonPropertyName("next_rank")]
    public string? NextRank { get; set; }

    [JsonPropertyName("Next_at")]
    public int? NextAt { get; set; }
}