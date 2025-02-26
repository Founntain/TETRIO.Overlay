using Microsoft.AspNetCore.Mvc;
using TetraLeague.Overlay.Network.Api.Tetrio;

namespace TetraLeague.Overlay.Controllers;

public class UserController : BaseController
{
    public UserController(TetrioApi api) : base(api)
    {
    }

    [HttpGet]
    [Route("{username}/stats")]
    public async Task<ActionResult> Stats(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return NotFound();

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
        if (string.IsNullOrWhiteSpace(username)) return NotFound();

        username = username.ToLower();

        var html = await System.IO.File.ReadAllTextAsync("wwwroot/web/user.html");

        html = html.Replace("{username}", username);

        return Content(html, "text/html");
    }
}