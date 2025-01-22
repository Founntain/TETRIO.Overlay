using System.Collections.Concurrent;
using System.Text.Json;
using TetraLeague.Overlay.Network.Api.Models;

namespace TetraLeague.Overlay.Network.Api;

public class TetrioApi : ApiBase
{
    private static readonly ConcurrentDictionary<string, (DateTimeOffset, Summary?)> SummaryCache = new();
    private static readonly ConcurrentDictionary<string, (DateTimeOffset, TetrioUser?)> UserCache = new();
    private static readonly ConcurrentDictionary<string, (DateTimeOffset, Models.TetraLeague?)> LeagueCache = new();
    private static readonly ConcurrentDictionary<string, (DateTimeOffset, Sprint?)> SprintCache = new();
    private static readonly ConcurrentDictionary<string, (DateTimeOffset, Blitz?)> BlitzCache = new();
    private static readonly ConcurrentDictionary<string, (DateTimeOffset, QuickPlay?)> ZenithCache = new();
    private static readonly ConcurrentDictionary<string, (DateTimeOffset, QuickPlay?)> ZenithExpertCache = new();

    private static readonly ConcurrentDictionary<string, (DateTimeOffset, ZenithRecords)> RecentZenithCache = new();
    private static readonly ConcurrentDictionary<string, (DateTimeOffset, ZenithRecords)> RecentZenithExpertCache = new();

    private string UserUrl => ApiBaseUrl + "users/{0}";
    private string SummariesUrl => ApiBaseUrl + "users/{0}/summaries";
    private string LeagueUrl => ApiBaseUrl + "users/{0}/summaries/league";
    private string SprintUrl => ApiBaseUrl + "users/{0}/summaries/40l";
    private string BlitzUrl => ApiBaseUrl + "users/{0}/summaries/blitz";
    private string ZenithUrl => ApiBaseUrl + "users/{0}/summaries/zenith";
    private string ZenithExpertUrl => ApiBaseUrl + "users/{0}/summaries/zenithex";
    private string RecentZenithUrl => ApiBaseUrl + "users/{0}/records/zenith/recent?limit=100";
    private string RecentZenithExpertUrl => ApiBaseUrl + "users/{0}/records/zenithex/recent?limit=100";

    public async Task<Summary?> GetUserSummaries(string username)
    {
        // Let's check the cache first
        if (SummaryCache.TryGetValue(username, out var data))
        {
            Console.WriteLine($"[SUMMARY] Found {username} in cache");

            // If the cache is still valid we return that
            if (data.Item1 >= DateTimeOffset.UtcNow)
            {
                Console.WriteLine("[SUMMARY] Found valid cache data to return");

                return data.Item2;
            }
        }

        Console.WriteLine($"[SUMMARY] Getting league stats for {username}, as nothing was found in the cache");

        try
        {
            var responseFromApi = await GetString(string.Format(SummariesUrl, username));

            if (responseFromApi == null) return default;

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<Summary>>(responseFromApi, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiResponse == null) return default;
            if (!apiResponse.Success) return default;

            if (apiResponse.Cache.Status == "hit")
            {
                Console.WriteLine("[SUMMARY] Cache hit... returning cache");

                SummaryCache.TryGetValue(username, out var result);

                if (result.Item2 != null) return result.Item2;

                Console.WriteLine("[SUMMARY] Cache hit, but nothing in there, fetching data again...");
            }

            if (apiResponse.Data == default) return default;

            Console.WriteLine("[SUMMARY] Updating cache and returning");

            // var cacheValidUntil = DateTimeOffset.FromUnixTimeMilliseconds(apiResponse.Cache.CacheUntil);
            var cacheValidUntil = DateTimeOffset.UtcNow.AddSeconds(30);

            if (SummaryCache.ContainsKey(username))
            {
                SummaryCache[username] = (cacheValidUntil, apiResponse.Data);

                return SummaryCache[username].Item2;
            }

            SummaryCache.TryAdd(username, (cacheValidUntil, apiResponse.Data));

            return apiResponse.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SUMMARY] ERROR: {ex.Message}");

            return default;
        }
    }

    public async Task<TetrioUser?> GetUserInformation(string username)
    {
        // Let's check the cache first
        if (UserCache.TryGetValue(username, out var data))
        {
            Console.WriteLine($"[USER] Found {username} in cache");

            // If the cache is still valid we return that
            if (data.Item1 >= DateTimeOffset.UtcNow)
            {
                Console.WriteLine("[USER] Found valid cache data to return");

                return data.Item2;
            }
        }

        Console.WriteLine($"[USER] Getting league stats for {username}, as nothing was found in the cache");

        try
        {
            var responseFromApi = await GetString(string.Format(UserUrl, username));

            if (responseFromApi == null) return default;

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<TetrioUser>>(responseFromApi, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiResponse == null) return default;
            if (!apiResponse.Success) return default;

            if (apiResponse.Cache.Status == "hit")
            {
                Console.WriteLine("[USER] Cache hit... returning cache");

                UserCache.TryGetValue(username, out var result);

                if (result.Item2 != null) return result.Item2;

                Console.WriteLine("[USER] Cache hit, but nothing in there, fetching data again...");
            }

            if (apiResponse.Data == default) return default;

            Console.WriteLine("[USER] Updating cache and returning");

            var cacheValidUntil = DateTimeOffset.FromUnixTimeMilliseconds(apiResponse.Cache.CacheUntil);

            if (UserCache.ContainsKey(username))
            {
                UserCache[username] = (cacheValidUntil, apiResponse.Data);

                return UserCache[username].Item2;
            }

            UserCache.TryAdd(username, (cacheValidUntil, apiResponse.Data));

            return apiResponse.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[USER] ERROR: {ex.Message}");

            return default;
        }
    }

    public async Task<Models.TetraLeague?> GetTetraLeagueStats(string username)
    {
        // Let's check the cache first
        if (LeagueCache.TryGetValue(username, out var data))
        {
            Console.WriteLine($"[TL] Found {username} in cache");

            // If the cache is still valid we return that
            if (data.Item1 >= DateTimeOffset.UtcNow)
            {
                Console.WriteLine("[TL] Found valid cache data to return");

                return data.Item2;
            }
        }

        Console.WriteLine($"[TL] Getting league stats for {username}, as nothing was found in the cache");

        try
        {
            var responseFromApi = await GetString(string.Format(LeagueUrl, username));

            if (responseFromApi == null) return default;

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<Models.TetraLeague>>(responseFromApi, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiResponse == null) return default;
            if (!apiResponse.Success) return default;

            if (apiResponse.Cache.Status == "hit")
            {
                Console.WriteLine("[TL] Cache hit... returning cache");

                LeagueCache.TryGetValue(username, out var result);

                if (result.Item2 != null) return result.Item2;

                Console.WriteLine("[TL] Cache hit, but nothing in there, fetching data again...");
            }

            if (apiResponse.Data == default) return default;

            Console.WriteLine("[TL] Updating cache and returning");

            // var cacheValidUntil = DateTimeOffset.FromUnixTimeMilliseconds(apiResponse.Cache.CacheUntil);
            var cacheValidUntil = DateTimeOffset.UtcNow.AddSeconds(30);

            if (LeagueCache.ContainsKey(username))
            {
                LeagueCache[username] = (cacheValidUntil, apiResponse.Data);

                return LeagueCache[username].Item2;
            }

            LeagueCache.TryAdd(username, (cacheValidUntil, apiResponse.Data));

            return apiResponse.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TL] ERROR: {ex.Message}");

            return default;
        }
    }

    public async Task<Sprint?> GetSprintStats(string username)
    {
        // Let's check the cache first
        if (SprintCache.TryGetValue(username, out var data))
        {
            Console.WriteLine($"[40L] Found {username} in cache");

            // If the cache is still valid we return that
            if (data.Item1 >= DateTimeOffset.UtcNow)
            {
                Console.WriteLine("[40L] Found valid cache data to return");

                return data.Item2;
            }
        }

        Console.WriteLine($"[40L] Getting 40L stats for {username}, as nothing was found in the cache");

        try
        {
            var responseFromApi = await GetString(string.Format(SprintUrl, username));

            if (responseFromApi == null) return default;

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<Sprint>>(responseFromApi, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiResponse == null) return default;
            if (!apiResponse.Success) return default;

            if (apiResponse.Cache.Status == "hit")
            {
                Console.WriteLine("[40L] Cache hit... returning cache");

                SprintCache.TryGetValue(username, out var result);

                if (result.Item2 != null) return result.Item2;

                Console.WriteLine("[40L] Cache hit, but nothing in there, fetching data again...");
            }

            if (apiResponse.Data == default) return default;

            Console.WriteLine("[40L] Updating cache and returning");

            var cacheValidUntil = DateTimeOffset.FromUnixTimeMilliseconds(apiResponse.Cache.CacheUntil);

            if (SprintCache.ContainsKey(username))
            {
                SprintCache[username] = (cacheValidUntil, apiResponse.Data);

                return SprintCache[username].Item2;
            }

            SprintCache.TryAdd(username, (cacheValidUntil, apiResponse.Data));

            return apiResponse.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[40L] ERROR: {ex.Message}");

            return default;
        }
    }

    public async Task<Blitz?> GetBlitzStats(string username)
    {
        // Let's check the cache first
        if (BlitzCache.TryGetValue(username, out var data))
        {
            Console.WriteLine($"[BLITZ] Found {username} in cache");

            // If the cache is still valid we return that
            if (data.Item1 >= DateTimeOffset.UtcNow)
            {
                Console.WriteLine("[BLITZ] Found valid cache data to return");

                return data.Item2;
            }
        }

        Console.WriteLine($"[BLITZ] Getting Blitz stats for {username}, as nothing was found in the cache");

        try
        {
            var responseFromApi = await GetString(string.Format(BlitzUrl, username));

            if (responseFromApi == null) return default;

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<Blitz>>(responseFromApi, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiResponse == null) return default;
            if (!apiResponse.Success) return default;

            if (apiResponse.Cache.Status == "hit")
            {
                Console.WriteLine("[BLITZ] Cache hit... returning cache");

                BlitzCache.TryGetValue(username, out var result);

                if (result.Item2 != null) return result.Item2;

                Console.WriteLine("[BLITZ] Cache hit, but nothing in there, fetching data again...");
            }

            if (apiResponse.Data == default) return default;

            Console.WriteLine("[BLITZ] Updating cache and returning");

            var cacheValidUntil = DateTimeOffset.FromUnixTimeMilliseconds(apiResponse.Cache.CacheUntil);

            if (BlitzCache.ContainsKey(username))
            {
                BlitzCache[username] = (cacheValidUntil, apiResponse.Data);

                return BlitzCache[username].Item2;
            }

            BlitzCache.TryAdd(username, (cacheValidUntil, apiResponse.Data));

            return apiResponse.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BLITZ] ERROR: {ex.Message}");

            return default;
        }
    }

    public async Task<QuickPlay?> GetZenithStats(string username, bool expert = false)
    {
        var prefix = expert ? "QP EX" : "QP";
        ;
        // Let's check the cache first
        if (!expert && ZenithCache.TryGetValue(username, out var normalData))
        {
            Console.WriteLine($"[{prefix}] Found {username} in cache");

            // If the cache is still valid we return that
            if (normalData.Item1 >= DateTimeOffset.UtcNow)
            {
                Console.WriteLine($"[{prefix}] Found valid cache data to return");

                return normalData.Item2;
            }
        }

        if (expert && ZenithExpertCache.TryGetValue(username, out var expertData))
        {
            Console.WriteLine($"[{prefix}] Found {username} in cache");

            // If the cache is still valid we return that
            if (expertData.Item1 >= DateTimeOffset.UtcNow)
            {
                Console.WriteLine($"[{prefix}] Found valid cache data to return");

                return expertData.Item2;
            }
        }

        Console.WriteLine($"[{prefix}] Getting Zenith stats for {username}, as nothing was found in the cache");

        try
        {
            var responseFromApi = await GetString(string.Format(expert ? ZenithExpertUrl : ZenithUrl, username));

            if (responseFromApi == null) return default;

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<QuickPlay>>(responseFromApi, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiResponse == null) return default;
            if (!apiResponse.Success) return default;

            if (apiResponse.Cache.Status == "hit")
            {
                Console.WriteLine($"[{prefix}] Cache hit... returning cache");

                if (!expert)
                {
                    ZenithCache.TryGetValue(username, out var result);

                    if (result.Item2 != null) return result.Item2;
                }
                else
                {
                    ZenithExpertCache.TryGetValue(username, out var result);

                    if (result.Item2 != null) return result.Item2;
                }

                Console.WriteLine($"[{prefix}] Cache hit, but nothing in there, fetching data again...");
            }

            if (apiResponse.Data == default) return default;

            Console.WriteLine($"[{prefix}] Updating cache and returning");

            // var cacheValidUntil = DateTimeOffset.FromUnixTimeMilliseconds(apiResponse.Cache.CacheUntil);
            var cacheValidUntil = DateTimeOffset.UtcNow.AddSeconds(30);

            if (!expert)
            {
                if (ZenithCache.ContainsKey(username))
                {
                    ZenithCache[username] = (cacheValidUntil, apiResponse.Data);

                    return ZenithCache[username].Item2;
                }

                ZenithCache.TryAdd(username, (cacheValidUntil, apiResponse.Data));
            }
            else
            {
                if (ZenithExpertCache.ContainsKey(username))
                {
                    ZenithExpertCache[username] = (cacheValidUntil, apiResponse.Data);

                    return ZenithExpertCache[username].Item2;
                }

                ZenithExpertCache.TryAdd(username, (cacheValidUntil, apiResponse.Data));
            }

            return apiResponse.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{prefix}] ERROR: {ex.Message}");

            return default;
        }
    }

    public async Task<ZenithRecords?> GetRecentZenithRecords(string username, bool expert = false)
    {
        var prefix = expert ? "QP EX" : "QP";
        ;
        // Let's check the cache first
        if (!expert && RecentZenithCache.TryGetValue(username, out var normalData))
        {
            Console.WriteLine($"[{prefix}] Found {username} in cache");

            // If the cache is still valid we return that
            if (normalData.Item1 >= DateTimeOffset.UtcNow)
            {
                Console.WriteLine($"[{prefix}] Found valid cache data to return");

                return normalData.Item2;
            }
        }

        if (expert && RecentZenithExpertCache.TryGetValue(username, out var expertData))
        {
            Console.WriteLine($"[{prefix}] Found {username} in cache");

            // If the cache is still valid we return that
            if (expertData.Item1 >= DateTimeOffset.UtcNow)
            {
                Console.WriteLine($"[{prefix}] Found valid cache data to return");

                return expertData.Item2;
            }
        }

        Console.WriteLine($"[{prefix}] Getting Zenith stats for {username}, as nothing was found in the cache");

        try
        {
            var responseFromApi = await GetString(string.Format(expert ? RecentZenithExpertUrl : RecentZenithUrl, username));

            if (responseFromApi == null) return default;

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<ZenithRecords?>>(responseFromApi, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiResponse == null) return default;
            if (!apiResponse.Success) return default;

            if (apiResponse.Cache.Status == "hit")
            {
                Console.WriteLine($"[{prefix}] Cache hit... returning cache");

                if (!expert)
                {
                    RecentZenithCache.TryGetValue(username, out var result);

                    if (result.Item2 != null) return result.Item2;
                }
                else
                {
                    RecentZenithExpertCache.TryGetValue(username, out var result);

                    if (result.Item2 != null) return result.Item2;
                }

                Console.WriteLine($"[{prefix}] Cache hit, but nothing in there, fetching data again...");
            }

            if (apiResponse.Data == default) return default;

            Console.WriteLine($"[{prefix}] Updating cache and returning");

            // var cacheValidUntil = DateTimeOffset.FromUnixTimeMilliseconds(apiResponse.Cache.CacheUntil);
            var cacheValidUntil = DateTimeOffset.UtcNow.AddSeconds(30);

            if (!expert)
            {
                if (RecentZenithCache.ContainsKey(username))
                {
                    RecentZenithCache[username] = (cacheValidUntil, apiResponse.Data);

                    return RecentZenithCache[username].Item2;
                }

                RecentZenithCache.TryAdd(username, (cacheValidUntil, apiResponse.Data));
            }
            else
            {
                if (RecentZenithExpertCache.ContainsKey(username))
                {
                    RecentZenithExpertCache[username] = (cacheValidUntil, apiResponse.Data);

                    return RecentZenithExpertCache[username].Item2;
                }

                RecentZenithExpertCache.TryAdd(username, (cacheValidUntil, apiResponse.Data));
            }

            return apiResponse.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{prefix}] ERROR: {ex.Message}");

            return default;
        }
    }
}