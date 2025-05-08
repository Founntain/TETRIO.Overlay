using System.Text.Json.Serialization;

namespace Tetrio.Foxhole.Network.Api.Tetrio.Models;

public class Badges
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("desc")]
    public string Description { get; set; }
}