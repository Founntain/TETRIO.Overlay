using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TetraLeague.Overlay.Network.Api.Discord;
using TetraLeague.Overlay.Network.Api.Tetrio;
using Tetrio.Overlay.Database;
using Tetrio.Overlay.Database.Entities;

namespace TetraLeague.Overlay.Controllers;

public class AuthController : MinBaseController
{
    private readonly DiscordApi _discordApi;
    private readonly EncryptionService _encryptionService;
    private readonly TetrioContext _context;

    public AuthController(TetrioApi api, DiscordApi discordApi, EncryptionService encryptionService, TetrioContext context) : base(api)
    {
        _discordApi = discordApi;
        _encryptionService = encryptionService;
        _context = context;
    }

    [HttpGet]
    [Route("discord")]
    public async Task<ActionResult<string>> DiscordAuth([FromQuery] string code, [FromQuery] string? state)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Authorization code is missing.");
        }

        if (string.IsNullOrEmpty(state))
        {
            return BadRequest("Missing code or state parameter.");
        }

        try
        {
            // Exchange the code for an access token
            var values = new Dictionary<string, string>
            {
                { "client_id", "1332751405374505154" },
                { "client_secret", "jULmwPf6l-VmZGSHl-Wzl-BQwAq7piIK" },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", "https://localhost:7053/auth/discord" }
            };

            using var client = new HttpClient();

            var tokenResult = await _discordApi.GetDiscordToken(values);

            if (!string.IsNullOrEmpty(tokenResult.ErrorMessage))
            {
                return BadRequest(tokenResult.ErrorMessage);
            }

            var discordUser = await _discordApi.GetDiscordUser(tokenResult.AccessToken);

            if (!string.IsNullOrEmpty(discordUser.ErrorMessage))
            {
                return BadRequest(discordUser.ErrorMessage);
            }

            var user = await Api.GetUserFromDiscordId(discordUser.Id);

            if (user == null)
            {
                return StatusCode(404, "User not found.");
            }

            var dbUser = await _context.Users.FirstOrDefaultAsync(x => x.DiscordId == discordUser.Id && x.TetrioId == user.User.Id);

            if (dbUser == null)
            {
                dbUser = new User()
                {
                    TetrioId = user.User.Id,
                    Username = user.User.Username,

                    SessionToken = Guid.NewGuid(),

                    DiscordId = discordUser.Id,
                    AccessToken = _encryptionService.EncryptWithIv(tokenResult.AccessToken),
                    RefreshToken = _encryptionService.EncryptWithIv(tokenResult.RefreshToken),
                    ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResult.ExpiresIn),
                };

                await _context.AddAsync(dbUser);
                await _context.SaveChangesAsync();
            }

            HttpContext.Response.Cookies.Append("username", dbUser.Username.ToString(), new CookieOptions
            {
                Path = "/",
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = dbUser.ExpiresAt,
            });

            HttpContext.Response.Cookies.Append("session_token", dbUser.SessionToken.ToString(), new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = dbUser.ExpiresAt,
            });

            // Return the user's Discord ID
            return Redirect(Uri.UnescapeDataString(state));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}