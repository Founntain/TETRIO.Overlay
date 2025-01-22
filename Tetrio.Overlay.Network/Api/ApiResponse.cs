using System.Text.Json.Serialization;

namespace TetraLeague.Overlay.Network.Api;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public object Error { get; set; }
    [JsonPropertyName("cache")]
    public CacheResponse Cache { get; set; }
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}

public class CacheResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("cached_at")]
    public long CachedAt { get; set; }
    [JsonPropertyName("cached_until")]
    public long CacheUntil { get; set; }
}