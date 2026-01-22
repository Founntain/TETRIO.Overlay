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
        var leaderboardDate = date.HasValue
            ? DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
            : DateTime.UtcNow;

        var leaderboard = await context.Leaderboards
            .Where(x => x.StartDate <= leaderboardDate && (x.EndDate == null || x.EndDate >= leaderboardDate))
            .Select(x => new
            {
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Name = x.Name,
                Description = x.Description,
                Leaderboard = x.Entries.Select(y => new
                {
                    Rank = y.User.TetrioRank,
                    Username = y.User.Username,
                    Score = y.Score
                }).OrderByDescending(y => y.Score).ToList()
            }).FirstOrDefaultAsync();

        if (leaderboard == null) return NotFound($"No leaderboard found for timestamp {leaderboardDate}");

        return Ok(new
            {
                leaderboard.StartDate,
                leaderboard.EndDate,
                StartedAtUnixSeconds = ((DateTimeOffset)DateTime.SpecifyKind(leaderboard.StartDate, DateTimeKind.Utc)).ToUnixTimeSeconds(),
                EndsAtUnixSeconds = leaderboard.EndDate.HasValue ? ((DateTimeOffset)DateTime.SpecifyKind(leaderboard.EndDate.Value, DateTimeKind.Utc)).ToUnixTimeSeconds() : -1,
                leaderboard.Name,
                leaderboard.Description,
                leaderboard.Leaderboard
            });
    }

    [HttpGet]
    [Route("getLeaderboardPosition/{username}")]
    public async Task<IActionResult> GetLeaderboardPosition(string username, DateTime? date = null)
    {
        username = username.ToLower();

        var user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);

        if(user == null) return NotFound($"User '{username}' not found");

        var leaderboardDate = date ?? DateTime.UtcNow;

        var leaderboardId = (await context.Leaderboards.Select(x => new {x.StartDate, x.EndDate, x.Id}).FirstOrDefaultAsync(x => x.StartDate <= leaderboardDate && (x.EndDate == null || x.EndDate >= leaderboardDate)))?.Id;

        if (leaderboardId == null) return NotFound($"Leaderboard not found for timestamp {leaderboardDate} not found");

        var userEntry = await context.LeaderboardEntries.FirstOrDefaultAsync(x => x.Leaderboard.Id == leaderboardId && x.User.Id == user.Id);

        if (userEntry == null) return NotFound("User is not placed on this leaderboard");

        var position = await context.LeaderboardEntries.CountAsync(x => x.Leaderboard.Id == leaderboardId && x.Score > userEntry.Score) + 1;

        return Ok(position);
    }
}