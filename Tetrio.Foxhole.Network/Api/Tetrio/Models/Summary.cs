using System.Text.Json.Serialization;

namespace Tetrio.Foxhole.Network.Api.Tetrio.Models;

public class Summary
{
    [JsonPropertyName("league")]
    public TetraLeague? TetraLeague { get; set; }

    [JsonPropertyName("zenith")]
    public QuickPlay? Zenith { get; set; }

    [JsonPropertyName("zenithex")]
    public QuickPlay? ZenithExpert { get; set; }

    [JsonPropertyName("40l")]
    public Sprint? Sprint { get; set; }

    [JsonPropertyName("blitz")]
    public Blitz? Blitz { get; set; }


}