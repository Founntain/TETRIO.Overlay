using System.Text.Json.Serialization;

namespace Tetrio.Foxhole.Network.Api.Tetrio.Models;

public class ZenithRecords
{
    [JsonPropertyName("entries")] public IList<Record> Entries { get; set; }
}