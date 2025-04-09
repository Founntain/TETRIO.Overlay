using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TetraLeague.Overlay.Network.Api.Tetrio;
using Tetrio.Overlay.Database;
using Tetrio.Overlay.Database.Entities;

namespace TetraLeague.Overlay.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class MinBaseController : ControllerBase
{
    protected readonly TetrioApi Api;

    public MinBaseController(TetrioApi api)
    {
        Api = api;
    }

    protected async Task<(bool IsAuthorized, int StatusCode, string ResponseText, User? User)> CheckIfAuthorized(TetrioContext? context)
    {
        if(context == null) return (false, StatusCodes.Status401Unauthorized, string.Empty, null);

        // Retrieve the session token from the cookie
        if (!HttpContext.Request.Cookies.TryGetValue("session_token", out string? sessionToken))
        {
            return (false, StatusCodes.Status401Unauthorized, "Session token not found.", null);
        }

        var token = Guid.Parse(sessionToken);
        var date = DateTimeOffset.UtcNow;

        var user = await context.Users.FirstOrDefaultAsync(x => x.SessionToken == token);

        if (user == null)
        {
            return (false, StatusCodes.Status401Unauthorized, "Invalid session token.", null);
        }

        if (user.ExpiresAt < date)
        {
            return (false, StatusCodes.Status401Unauthorized, "Session token expired.", null);
        }

        return (true, StatusCodes.Status200OK, $"User {user.Username} authorized successfully", user);
    }

    protected void ResetCookies()
    {
#if DEBUG
        HttpContext.Response.Cookies.Append("username",string.Empty, new CookieOptions
        {
            Path = "/",
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(-3),
        });

        HttpContext.Response.Cookies.Append("session_token", string.Empty, new CookieOptions
        {
            Path = "/",
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(-3),
        });
#else
            HttpContext.Response.Cookies.Append("username", string.Empty, new CookieOptions
            {
                Path = "/",
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = ".founntain.dev",
                Expires = DateTime.UtcNow.AddDays(-3),
            });

            HttpContext.Response.Cookies.Append("session_token", string.Empty, new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = ".founntain.dev",
                Expires = DateTime.UtcNow.AddDays(-3)
            });
#endif
    }
}