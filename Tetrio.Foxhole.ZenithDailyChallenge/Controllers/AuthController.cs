using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Database.Entities;
using Tetrio.Foxhole.Network.Api.Discord;
using Tetrio.Foxhole.Network.Api.Tetrio;
using Tetrio.Foxhole.Network.Api.Tetrio.Models;

namespace Tetrio.Zenith.DailyChallenge.Controllers;

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

        if (System.IO.File.Exists("/run/secrets/ZENITH_DAILY_CHALLENGE_DISCORD_CLIENT_SECRET"))
        {
            _discordClientSecret = System.IO.File.ReadAllText("/run/secrets/ZENITH_DAILY_CHALLENGE_DISCORD_CLIENT_SECRET");

            Console.WriteLine("loaded discord-client-secrets from secrets");

            return;
        }

        var discordClientSecret = Environment.GetEnvironmentVariable("ZENITH_DAILY_CHALLENGE_DISCORD_CLIENT_SECRET");

        if (string.IsNullOrEmpty(discordClientSecret))
        {
            throw new ArgumentException("ZENITH_DAILY_CHALLENGE_DISCORD_CLIENT_SECRET environment variable is not set.");
        }

        _discordClientSecret = discordClientSecret;

        Console.WriteLine("loaded ZENITH_DAILY_CHALLENGE_DISCORD_CLIENT_SECRET from environment variable");
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
                { "redirect_uri", "https://tetrio.founntain.dev/auth/discord" }
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

            var tetrioUserResult = await Api.GetUserFromDiscordId(discordUser.Id);

            var tetrioUser = tetrioUserResult?.Users.FirstOrDefault();

            if (tetrioUser == null)
            {
                return StatusCode(404, "User not found. This could also mean, that your discord account is not linked to a TETR.IO account or is not publicly visible.");
            }

            // Check if we got a user already for the given TETR.IO User that we loaded with the discord ID;
            var dbUser = await _context.Users.FirstOrDefaultAsync(x => x.TetrioId == tetrioUser.Id);

            if (dbUser == null)
            {
                dbUser = new User()
                {
                    TetrioId = tetrioUser.Id,
                    Username = tetrioUser.Username,

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
                if(tetrioUser.Username != dbUser.Username)
                    dbUser.Username = tetrioUser.Username;

                if (dbUser.DiscordId != discordUser.Id)
                    dbUser.DiscordId = discordUser.Id;

                dbUser.SessionToken = Guid.NewGuid();
                dbUser.AccessToken = _encryptionService.EncryptWithIv(tokenResult.AccessToken);
                dbUser.RefreshToken = _encryptionService.EncryptWithIv(tokenResult.RefreshToken);
                dbUser.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResult.ExpiresIn);

                await _context.SaveChangesAsync();
            }

#if DEBUG
            HttpContext.Response.Cookies.Append("username", dbUser.Username, new CookieOptions
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

            return Redirect(Uri.UnescapeDataString(state));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult> GetProfileFromAuthorization()
    {
        var authResult = await CheckIfAuthorized(_context);

        if (!authResult.IsAuthorized)
        {
            ResetCookies();

            return StatusCode(authResult.StatusCode, $"{authResult.StatusCode} - Unauthorized. Reason: {authResult.ResponseText}");
        }

        var user = authResult.User;

        if (user == null) return Ok("You are not authorized to submit daily challenges, please log in again and try again");

        var userInfo = await Api.GetUserInformation(user.TetrioId);

        if(userInfo == default) return NotFound("Userinfo could not be fetched from TETR.IO API");

        if(userInfo.Username != user.Username)
        {
            user.Username = userInfo.Username;

            await _context.SaveChangesAsync();
        }

        return Ok(new SlimUserInfo
        {
            Username = user.Username,
            UserId = user.TetrioId,
            AvatarRevision = userInfo.Avatar,
            BannerRevision = userInfo.Banner,
        });
    }

    [HttpPost]
    [Route("logout")]
    public async Task<ActionResult> Logout()
    {
        var authResult = await CheckIfAuthorized(_context);

        if (!authResult.IsAuthorized)
        {
            ResetCookies();

            return StatusCode(authResult.StatusCode, $"{authResult.StatusCode} - Unauthorized. Reason: {authResult.ResponseText}");
        }

        var user = authResult.User;

        if (user == null)
        {
            ResetCookies();

            return Ok("You are not authorized or user not found");
        }

        user.SessionToken = null;
        user.AccessToken = null;
        user.RefreshToken = null;
        user.ExpiresAt = null;

        await _context.SaveChangesAsync();

        return Ok("Logged out successfully");
    }
}