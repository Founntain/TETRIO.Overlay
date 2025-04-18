using Microsoft.AspNetCore.Mvc;
using TetraLeague.Overlay.Network.Api.Tetrio;

namespace TetraLeague.Overlay.Controllers;

[Route("[controller]")]
public class BlitzController(TetrioApi api) : BaseController(api)
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("This Endpoint is for Blitz Overlays");
    }

    [HttpGet]
    [Route("{username}")]
    public async Task<ActionResult> Web(string username)
    {
        username = username.ToLower();

        var html = await System.IO.File.ReadAllTextAsync("wwwroot/web/blitz.html");

        html = html.Replace("{mode}", ControllerContext.ActionDescriptor.ControllerName);

        html = html.Replace("{username}", username);

        return Content(html, "text/html");
    }

    [HttpGet]
    [Route("{username}/stats")]
    public ActionResult GetStats(string username)
    {
        username = username.ToLower();

        var userStats = Api.GetUserInformation(username);
        var stats = Api.GetBlitzStats(username);

        if(userStats?.Result == null) return NotFound("User Stats could not be fetched from the TETR.IO API");
        if(stats?.Result?.Record?.Results?.Stats?.Finesse == null) return NotFound("Blitz stats could not be fetched from the TETR.IO API");

        return Ok(new
        {
            Country = userStats.Result.Country,
            Score = stats.Result.Record.Results.Stats.Score,
            Pps = stats.Result.Record.Results.Aggregatestats.Pps,
            Kpp = (double)stats.Result.Record.Results.Stats.Inputs! / (double)stats.Result.Record.Results.Stats.Piecesplaced!,
            Sps = (double)stats.Result.Record.Results.Stats.Score! / (double)stats.Result.Record.Results.Stats.Piecesplaced!,
            Finesse = stats.Result.Record.Results.Stats.Finesse!.Faults,
            GlobalRank = stats.Result.Rank,
            LocalRank = stats.Result.RankLocal
        });
    }
}