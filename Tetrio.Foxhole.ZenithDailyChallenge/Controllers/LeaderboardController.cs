using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Network.Api.Tetrio;

namespace Tetrio.Zenith.DailyChallenge.Controllers;

public class LeaderboardController(TetrioApi api, TetrioContext context) : BaseController(api)
{
    [HttpGet]
    public async Task<IActionResult> GetLeaderboard(DateTime? date = null)
    {
        var leaderboardDate = date != null
            ? DateOnly.FromDateTime(date.Value)
            : DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);

        var leaderboard = await context.Leaderboards.Select(x => new
        {
            StartDate =x.StartDate,
            EndDate = x.EndDate,
            Name = x.Name,
            Description = x.Description,
            Entries = x.Entries.Select(y => new
            {
                y.User.Username,
                y.Score
            })
        }).FirstOrDefaultAsync(x => x.StartDate >= leaderboardDate && x.EndDate <= leaderboardDate);

        return Ok(leaderboard);
    }
}