using Microsoft.AspNetCore.Mvc;
using TetraLeague.Overlay.Network.Api.Models;
using TetraLeague.Overlay.Network.Api.Tetrio;

namespace TetraLeague.Overlay.Controllers;

public class AchievementController : BaseController
{
    public AchievementController(TetrioApi api) : base(api)
    {

    }

    [HttpGet]
    [Route("{id}/{username}")]
    public async Task<ActionResult> Web(string id, string username)
    {
        username = username.ToLower();

        var html = await System.IO.File.ReadAllTextAsync("wwwroot/web/achievement.html");

        html = html.Replace("{mode}", ControllerContext.ActionDescriptor.ControllerName);

        html = html.Replace("{username}", username);
        html = html.Replace("{achievement}", id);

        return Content(html, "text/html");
    }

    [HttpGet]
    [Route("{id}/{username}/data")]
    public async Task<IActionResult> GetAchievement([FromRoute] string id, [FromRoute] string username)
    {
        username = username.ToLower();

        var achievement = await Api.GetAchievement(id);

        if (achievement == null)
        {
            return NotFound();
        }

        var userIndex = achievement.Leaderboard.FindIndex(x => x.User.Username == username);

        if (userIndex == -1)
        {
            return NotFound();
        }

        AchievementLeaderboardEntry? leaderBoardEntryBefore = null;
        AchievementLeaderboardEntry userEntry = achievement.Leaderboard[userIndex];
        AchievementLeaderboardEntry? leaderBoardEntryAfter = null;

        if (userIndex != 0 && achievement.Leaderboard.Count > 1)
        {
            leaderBoardEntryBefore = achievement.Leaderboard[userIndex - 1];
        }

        if (userIndex != achievement.Leaderboard.Count - 1)
        {
            leaderBoardEntryAfter = achievement.Leaderboard[userIndex + 1];
        }

        return Ok(new
        {
            AchievementName = achievement.AchievementInfo.Name,
            Before = new
            {
                Username = leaderBoardEntryBefore?.User.Username,
                Value = leaderBoardEntryBefore?.Value,
                AdditionalValue = leaderBoardEntryBefore?.AdditionalValue,
                Country = leaderBoardEntryBefore?.User.Country,
                Rank = userIndex
            },
            User = new
            {
                Username = userEntry.User.Username,
                Value = userEntry.Value,
                AdditionalValue = userEntry.AdditionalValue,
                Country = userEntry.User.Country,
                Rank = userIndex + 1
            },
            After = new
            {
                Username = leaderBoardEntryAfter?.User.Username,
                Value = leaderBoardEntryAfter?.Value,
                AdditionalValue = leaderBoardEntryAfter?.AdditionalValue,
                Country = leaderBoardEntryAfter?.User.Country,
                Rank = userIndex + 2
            },
        });
    }
}