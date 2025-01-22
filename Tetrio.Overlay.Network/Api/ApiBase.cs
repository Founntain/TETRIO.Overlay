namespace TetraLeague.Overlay.Network.Api;

public abstract class ApiBase
{
    protected const string ApiBaseUrl = "https://ch.tetr.io/api/";

    protected async Task<string?> GetString(string url)
    {
        try
        {
            using var client = new HttpClient();

            var uri = new Uri(url);

            client.DefaultRequestHeaders.Add("X-Session-ID", Guid.NewGuid().ToString());

            return await client.GetStringAsync(uri);
        }
        catch (Exception _)
        {
            return null;
        }
    }
}