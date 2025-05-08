using System.Text.Json.Serialization;

namespace Tetrio.Foxhole.Network.Api.Discord.Models;

public class DiscordUserResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("discriminator")]
    public string Discriminator { get; set; }

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; }

    public string? ErrorMessage { get; set; }
}