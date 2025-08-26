using Microsoft.AspNetCore.Mvc;
using Tetrio.Foxhole.Network.Api.Tetrio;
using Tetrio.Foxhole.Network.Api.Tetrio.Models;

namespace Tetrio.Foxhole.Backend.Base.Controllers;

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

    protected async Task<SlimUserInfo?> GetTetrioUserInformation(string username)
    {
        var user = await Api.GetUserInformation(username);

        if(user == default) return null;

        return new SlimUserInfo
        {
            Username = user.Username,
            Avatar = $"https://tetr.io/user-content/avatars/{user.Id}.jpg?rv={user.Avatar}",
            AvatarRevision = user.Avatar,
            Banner = $"https://tetr.io/user-content/banners/{user.Id}.jpg?rv={user.Banner}",
            BannerRevision = user.Banner,
        };
    }
}