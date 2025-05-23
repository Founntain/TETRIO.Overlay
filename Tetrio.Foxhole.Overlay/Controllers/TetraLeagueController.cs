﻿using Microsoft.AspNetCore.Mvc;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Network.Api.Tetrio;

namespace Tetrio.Foxhole.Overlay.Controllers;

public class TetraLeagueController(TetrioApi api) : BaseController(api)
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("This Endpoint is for Tetra League Overlays");
    }

    [HttpGet]
    [Route("stats/{username}")]
    public async Task<ActionResult> Stats(string username, string? textcolor = null, string? backgroundColor = null, bool displayUsername = true)
    {
        return await StatsNew(username, textcolor, backgroundColor, displayUsername);
    }

    [HttpGet]
    [Route("{username}")]
    public async Task<ActionResult> StatsNew(string username, string? textcolor = null, string? backgroundColor = null, bool displayUsername = true)
    {
        username = username.ToLower();

        var html = await System.IO.File.ReadAllTextAsync("wwwroot/web/league.html");

        html = html.Replace("{username}", username);

        return Content(html, "text/html");
    }

    [HttpGet]
    [Route("stats/{username}/web")]
    public async Task<ActionResult> WebAlt(string username, string? textcolor = null, string? backgroundColor = null)
    {
        return await StatsNew(username, textcolor, backgroundColor);
    }

    [HttpGet]
    [Route("{username}/stats")]
    public async Task<ActionResult> GetStats(string username)
    {
        username = username.ToLower();

        var user = await Api.GetUserInformation(username);
        var stats = await Api.GetTetraLeagueStats(username);

        if(user == null || stats == null) return NotFound();

        return Ok(new
        {
            Username= user.Username,
            Country = user.Country,
            Tr = stats.Tr,
            Rank = stats.Rank,
            Apm = stats.Apm,
            Pps = stats.Pps,
            Vs = stats.Vs,
            GlobalRank = stats.StandingGlobal,
            CountryRank = stats.StandingLocal,
            TopRank = stats.TopRank,
            PrevRank = stats.PrevRank,
            PrevAt = stats.PrevAt,
            NextRank = stats.NextRank,
            NextAt = stats.NextAt,
            GamesPlayed = stats.Gamesplayed
        });
    }
}