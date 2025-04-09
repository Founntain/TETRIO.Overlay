using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TetraLeague.Overlay.Network.Api.Discord;
using TetraLeague.Overlay.Network.Api.Tetrio;
using TetraLeague.Overlay.Network.Api.Tetrio.Models;
using Tetrio.Overlay.Database;
using Tetrio.Overlay.Database.Entities;

namespace TetraLeague.Overlay.Controllers;

public class AuthController : MinBaseController
{
    private readonly DiscordApi _discordApi;
    private readonly EncryptionService _encryptionService;
    private readonly TetrioContext _context;

    private readonly string _discordClientSecret;

    public AuthController(TetrioApi api, DiscordApi discordApi, EncryptionService encryptionService, TetrioContext context) : base(api)
    {
        _discordApi = discordApi;
        _encryptionService = encryptionService;
        _context = context;

        if (System.IO.File.Exists("/run/secrets/discord-client-secret"))
        {
            _discordClientSecret = System.IO.File.ReadAllText("/run/secrets/discord-client-secret");

            Console.WriteLine("loaded discord-client-secrets from secrets");

            return;
        }

        var discordClientSecret = Environment.GetEnvironmentVariable("discord-client-secret");

        if (string.IsNullOrEmpty(discordClientSecret))
        {
            throw new ArgumentException("discord-client-secret environment variable is not set.");
        }

        _discordClientSecret = discordClientSecret;

        Console.WriteLine("loaded discord-client-secret from environment variable");
    }

    [HttpGet]
    [Route("discord")]
    public async Task<ActionResult> DiscordAuth([FromQuery] string code, [FromQuery] string? state)
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
                { "client_secret", _discordClientSecret },
                { "grant_type", "authorization_code" },
                { "code", code },
                #if DEBUG
                { "redirect_uri", "https://localhost:7053/auth/discord" }
                #else
                { "redirect_uri", "https://teto.founntain.dev/auth/discord" }
                #endif
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
            else
            {
                dbUser.SessionToken = Guid.NewGuid();
                dbUser.AccessToken = _encryptionService.EncryptWithIv(tokenResult.AccessToken);
                dbUser.RefreshToken = _encryptionService.EncryptWithIv(tokenResult.RefreshToken);
                dbUser.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResult.ExpiresIn);

                await _context.SaveChangesAsync();
            }

#if DEBUG
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
#else
            HttpContext.Response.Cookies.Append("username", dbUser.Username.ToString(), new CookieOptions
            {
                Path = "/",
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = ".founntain.dev",
                Expires = dbUser.ExpiresAt,
            });

            HttpContext.Response.Cookies.Append("session_token", dbUser.SessionToken.ToString(), new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = ".founntain.dev",
                Expires = dbUser.ExpiresAt,
            });
#endif

            // Return the user's Discord ID
            return Redirect(Uri.UnescapeDataString(state));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> GetProfileFromAuthorization()
    {
        var authResult = await CheckIfAuthorized(_context);

        if (!authResult.IsAuthorized)
        {
            ResetCookies();

            return StatusCode(authResult.StatusCode, $"{authResult.StatusCode} - Unauthorized. Reason: {authResult.ResponseText}");
        }

        var user = authResult.User;

        if (user == null) return Ok("You are not authorized to submit daily challenges, please log in again and try again");

        var userInfo = await Api.GetUserInformation(authResult.User.Username);

        if(userInfo == default) return null;

        return Ok(new SlimUserInfo
        {
            Username = user.Username,
            Avatar = $"https://tetr.io/user-content/avatars/{userInfo.Id}.jpg?rv={userInfo.Avatar}",
            Banner = $"https://tetr.io/user-content/banners/{userInfo.Id}.jpg?rv={userInfo.Banner}",
        });
    }
}