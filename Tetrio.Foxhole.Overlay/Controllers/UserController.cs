using Microsoft.AspNetCore.Mvc;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Network.Api.Tetrio;

namespace Tetrio.Foxhole.Overlay.Controllers;

public class UserController(TetrioApi api, TetrioContext context) : BaseController(api)
{
    [HttpGet]
    [Route("{username}/stats")]
    public async Task<ActionResult> Stats(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        var userData = await Api.GetUserInformation(username);
        var userSummaryData = await Api.GetUserSummaries(username);

        var data = new
        {
            Badges = userData?.Badges?.Select(x => x.Id),
            SummaryData = userSummaryData
        };

        return Ok(data);
    }

    [HttpGet]
    [Route("{username}")]
    public async Task<ActionResult> View(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();

        username = username.ToLower();

        var html = await System.IO.File.ReadAllTextAsync("wwwroot/web/user.html");

        html = html.Replace("{username}", username);

        return Content(html, "text/html");
    }
}