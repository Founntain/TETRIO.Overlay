using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Network.Api.Tetrio;

namespace Tetrio.Zenith.DailyChallenge.Controllers;

public class ShareController(TetrioApi api, TetrioContext context) : BaseController(api)
{
    [HttpGet]
    [Route("{username}")]
    public async Task<ActionResult> ShareUserProfile(string username)
    {
        username = username.ToLower();

        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

        if (user == null) return NotFound($"User {username} not found");

        var runs = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id).CountAsync();
        var today = DateTime.UtcNow;
        var daysSinceMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var startOfWeek = today.Date.AddDays(-daysSinceMonday);

        var topRun = await context.Runs.AsNoTracking().Where(x => x.User.Id == user.Id && x.PlayedAt >= startOfWeek).OrderByDescending(x => x.Altitude).FirstOrDefaultAsync();

        var topRunString = topRun == null ? "" : $" - Top run this week: {Math.Round(topRun.Altitude, 2)} M";

        var result = string.Empty;

        result += $"""
                  <html lang="en">
                      <head>
                       <meta property="og:title" content="{user.Username.ToUpper()}'s profile" />
                        <meta property="og:description" content="{user.Score} Score - {runs} Runs{topRunString}" />
                        <meta property="og:image" content="https://tetr.io/user-content/avatars/{user.TetrioId}.jpg" />
                        <meta property="og:image:type" content="image/jpeg" />
                        <meta property="og:image:width" content="300" />
                        <meta property="og:image:height" content="300" />
                        <meta property="og:url" content="https://zenith.founntain.dev/u/{user.Username}" />
                        <meta property="og:site_name" content="Zenith Daily Challenge" />
                        <meta content="#92affc" property="og:theme_color" />
                        <meta content="#92affc" property="theme_color" />
                        
                        <script>
                          window.location.href = "https://zenith.founntain.dev/u/{user.Username}";
                        </script>
                      </head>
                      <body>
                        <p>Redirecting to <a href="https://zenith.founntain.dev/u/{user.Username}">https://zenith.founntain.dev/u/{user.Username}</a>...</p>
                      </body>
                    "</html>"
                  """;

        return Content(result, "text/html");
    }

    [HttpGet]
    [Route("{username}/run/{runId}")]
    public async Task<ActionResult> ShareRun(string username, string runId)
    {
        username = username.ToLower();

        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

        if (user == null) return NotFound($"User {username} not found");

        var run = await context.Runs.AsNoTracking().FirstOrDefaultAsync(x => x.TetrioId == runId);

        if (run == null) return NotFound($"Run {runId} not found");

        var result = string.Empty;

        var floor = (int) run.GetFloor();

        var mods = string.Join(' ', run.Mods.Split(' ').Select(x => x.ToUpper()));

        var modString = string.IsNullOrWhiteSpace(run.Mods) ? "" : $" - Mods: {mods}";

        result += $"""
                     <html lang="en">
                         <head>
                           <meta property="og:title" content="{Math.Round(run.Altitude, 2)} M by {user.Username.ToUpper()}" />
                           <meta property="og:description" content="PPS: {Math.Round(run.Pps, 2)} - APM: {Math.Round(run.Apm, 2)} - VS: {Math.Round(run.Vs, 2)}{modString}"/>
                           <meta property="og:image" content="https://tetr.io/res/bg/zenith/{floor}fa.jpg" />
                           <meta property="og:url" content="https://zenith.founntain.dev/u/{user.Username}/run/{runId}" />
                           <meta property="og:site_name" content="Zenith Daily Challenge" />
                           <meta content="#92affc" property="og:theme_color" />
                           <meta content="#92affc" property="theme_color" />
                           <script>
                             window.location.href = "https://zenith.founntain.dev/u/{user.Username}/run/{runId}";
                           </script>
                         </head>
                         <body>
                           <p>Redirecting to <a href="https://zenith.founntain.dev/u/{user.Username}/run/{runId}">https://zenith.founntain.dev/u/{user.Username}/run/{runId}</a>...</p>
                         </body>
                     "</html>"
                     """;

        return Content(result, "text/html");
    }
}