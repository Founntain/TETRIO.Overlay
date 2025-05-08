using System.Text.Json.Serialization;

namespace Tetrio.Foxhole.Network.Api.Discord.Models;

public class DiscordTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public uint ExpiresIn { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; }

    public string? ErrorMessage { get; set; }
}