using System.Text.Json.Serialization;

namespace Tetrio.Foxhole.Network.Api.Tetrio.Models;

public class TetrioUser
{
    [JsonPropertyName("_id")]
    public string Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("xp")]
    public double Xp { get; set; }

    [JsonPropertyName("gamesplayed")]
    public int? GamesPlayed { get; set; }

    [JsonPropertyName("gameswon")]
    public int? GamesWon { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("avatar_revision")]
    public double? Avatar { get; set; }

    [JsonPropertyName("banner_revision")]
    public double? Banner { get; set; }

    [JsonPropertyName("supporter")]
    public bool Supporter { get; set; }

    [JsonPropertyName("supportertier")]
    public int? SupporterTier { get; set; }

    [JsonPropertyName("badges")]
    public List<Badges>? Badges { get; set; }
}