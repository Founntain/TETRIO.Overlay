using Microsoft.AspNetCore.Mvc;
using TetraLeague.Overlay.Generator;
using TetraLeague.Overlay.Network.Api;

namespace TetraLeague.Overlay.Controllers;

[Route("[controller]")]
public class SprintController : BaseController
{
    public SprintController(TetrioApi api) : base(api) { }

    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("This Endpoint is for 40L Overlays");
    }

    [HttpGet]
    [Route("{username}")]
    public async Task<ActionResult> Web(string username)
    {
        username = username.ToLower();

        var html = await System.IO.File.ReadAllTextAsync("wwwroot/web/sprint.html");

        html = html.Replace("{mode}", ControllerContext.ActionDescriptor.ControllerName);

        html = html.Replace("{username}", username);

        return Content(html, "text/html");
    }

    [HttpGet]
    [Route("{username}/stats")]
    public async Task<ActionResult> GetStats(string username)
    {
        username = username.ToLower();

        var userStats = _api.GetUserInformation(username);
        var stats = _api.GetSprintStats(username);

        return Ok(new
        {
            Country = userStats.Result.Country,
            Time = stats.Result.Record.Results.Stats.Finaltime,
            TimeString = TimeSpan.FromMilliseconds(stats.Result.Record.Results.Stats.Finaltime.Value).ToString(@"mm\:ss\.fff"),
            Pps = stats.Result.Record.Results.Aggregatestats.Pps,
            Kpp = (double)stats.Result.Record.Results.Stats.Inputs! / (double)stats.Result.Record.Results.Stats.Piecesplaced!,
            kps = (stats.Result.Record.Results.Stats.Inputs / (stats.Result.Record.Results.Stats.Finaltime / 1000)),
            Finesse = stats.Result.Record.Results.Stats.Finesse.Faults,
            GlobalRank = stats.Result.Rank,
            LocalRank = stats.Result.RankLocal
        });
    }
}