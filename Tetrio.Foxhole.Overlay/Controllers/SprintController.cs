using Microsoft.AspNetCore.Mvc;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Network.Api.Tetrio;

namespace Tetrio.Foxhole.Overlay.Controllers;

[Route("[controller]")]
public class SprintController(TetrioApi api) : BaseController(api)
{
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

        var userStats = await Api.GetUserInformation(username);
        var stats = await Api.GetSprintStats(username);

        if(userStats == null) return NotFound("User Stats could not be fetched from the TETR.IO API");
        if(stats?.Record?.Results.Stats == null) return NotFound("Blitz stats could not be fetched from the TETR.IO API");

        return Ok(new
        {
            Country = userStats.Country,
            Time = stats.Record.Results.Stats.Finaltime,
            TimeString = TimeSpan.FromMilliseconds(stats.Record.Results.Stats.Finaltime!.Value).ToString(@"mm\:ss\.fff"),
            Pps = stats.Record.Results.Aggregatestats.Pps,
            Kpp = (double)stats.Record.Results.Stats.Inputs! / (double)stats.Record.Results.Stats.Piecesplaced!,
            kps = (stats.Record.Results.Stats.Inputs / (stats.Record.Results.Stats.Finaltime / 1000)),
            Finesse = stats.Record.Results.Stats.Finesse!.Faults,
            GlobalRank = stats.Rank,
            LocalRank = stats.RankLocal
        });
    }
}