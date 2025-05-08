using System.Net.Http.Headers;
using System.Text.Json;
using Tetrio.Foxhole.Network.Api.Discord.Models;

namespace Tetrio.Foxhole.Network.Api.Discord;

public class DiscordApi
{
    public async Task<DiscordTokenResponse> GetDiscordToken(Dictionary<string, string> values)
    {
        using var client = new HttpClient();

        var content = new FormUrlEncodedContent(values);

        var tokenResponse = await client.PostAsync("https://discord.com/api/oauth2/token", content);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            return new() { ErrorMessage = $"Failed to exchange authorization code.{Environment.NewLine}{Environment.NewLine}{tokenResponse.ReasonPhrase}{Environment.NewLine}{Environment.NewLine}{await tokenResponse.Content.ReadAsStringAsync()}" };
        }

        var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
        var tokenResult = JsonSerializer.Deserialize<DiscordTokenResponse>(tokenContent);

        if (tokenResult == null)
        {
            return new() { ErrorMessage = $"Failed to retrieve Discord token.{Environment.NewLine}{Environment.NewLine}{tokenResponse.ReasonPhrase}" };
        }

        return tokenResult;
    }

    public async Task<DiscordUserResponse> GetDiscordUser(string accessToken)
    {
        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var userResponse = await client.GetAsync("https://discord.com/api/users/@me");

        if (!userResponse.IsSuccessStatusCode)
        {
            return new() { ErrorMessage = $"Failed to fetch user information.{Environment.NewLine}{Environment.NewLine}{userResponse.ReasonPhrase}" };
        }

        var userContent = await userResponse.Content.ReadAsStringAsync();
        var discordUser = JsonSerializer.Deserialize<DiscordUserResponse>(userContent);

        if (discordUser == null)
        {
            return new() { ErrorMessage = $"Failed to retrieve Discord user.{Environment.NewLine}{Environment.NewLine}{userResponse.ReasonPhrase}" };
        }

        return discordUser;
    }
}