using Microsoft.AspNetCore.Mvc;
using TetraLeague.Overlay.Network.Api.Tetrio;

namespace TetraLeague.Overlay.Controllers;

public class BaseController(TetrioApi api) : MinBaseController(api)
{
    [HttpGet]
    [Route("{username}/web")]
    public async Task<ActionResult> Web(string username, string? textcolor = null, string? backgroundColor = null, bool displayUsername = true)
    {
        username = username.ToLower();

        var html = await System.IO.File.ReadAllTextAsync("Web/overlay.html");

        html = html.Replace("{mode}", ControllerContext.ActionDescriptor.ControllerName);

        html = html.Replace("{username}", username);
        html = html.Replace("{textColor}", textcolor ?? "FFFFFF");
        html = html.Replace("{backgroundColor}", backgroundColor ?? "00FFFFFF");
        html = html.Replace("{displayUsername}", displayUsername.ToString());

        return Content(html, "text/html");
    }
}