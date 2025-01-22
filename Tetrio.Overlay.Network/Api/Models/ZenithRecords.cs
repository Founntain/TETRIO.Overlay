using System.Text.Json.Serialization;

namespace TetraLeague.Overlay.Network.Api.Models;

public class ZenithRecords
{
    [JsonPropertyName("entries")] public IList<Record> Entries { get; set; }
}