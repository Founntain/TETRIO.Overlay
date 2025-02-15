using System.Text.Json.Serialization;

namespace TetraLeague.Overlay.Network.Api.Models;

public class Achievement : ApiRecord
{
    [JsonPropertyName("achievement")] public AchievementInfo AchievementInfo { get; set; }
    [JsonPropertyName("leaderboard")] public List<AchievementLeaderboardEntry> Leaderboard { get; set; }
    [JsonPropertyName("cutoffs")] public AchievementInfo Cuttoffs { get; set; }
}

public class AchievementLeaderboardEntry
{
    [JsonPropertyName("u")] public AchievementUser User { get; set; }
    [JsonPropertyName("v")] public float Value { get; set; }
    [JsonPropertyName("a")] public float? AdditionalValue { get; set; }
    [JsonPropertyName("t")] public string TimeUpdated { get; set; }
}

public class AchievementUser
{
    [JsonPropertyName("_id")] public string Id { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; }
    [JsonPropertyName("role")] public string Role { get; set; }
    [JsonPropertyName("supporter")] public bool Supporter { get; set; }
    [JsonPropertyName("country")] public string? Country { get; set; }
}

public class AchievementInfo
{
    [JsonPropertyName("name")] public string Name { get; set; }
}