using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TetraLeague.Overlay.Network.Api;
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

    protected async Task<ActionResult> CheckIfAuthorized(TetrioContext? context)
    {
        if(context == null) return Unauthorized("No database context available.");

        // Retrieve the session token from the cookie
        if (!HttpContext.Request.Cookies.TryGetValue("session_token", out string? sessionToken))
        {
            return Unauthorized("Session token is missing.");
        }

        var token = Guid.Parse(sessionToken);
        var date = DateTimeOffset.UtcNow;

        var user = await context.Users.FirstOrDefaultAsync(x => x.SessionToken == token);

        if (user == null || user.ExpiresAt < date)
        {
            return Unauthorized("Invalid session token.");
        }

        return Ok(user);
    }
}