using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tetrio.Foxhole.Backend.Base.Controllers;
using Tetrio.Foxhole.Database;
using Tetrio.Foxhole.Network.Api.Tetrio;

namespace Tetrio.Zenith.DailyChallenge.Controllers;

[Route("zenith/weekly")]
public class WeeklyController(TetrioApi api, TetrioContext context) : BaseController(api)
{
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetWeeklyChallenge()
    {
        var day = DateOnly.FromDateTime(DateTime.Now);

        // check if the day is the start of the week if not get the first day of the week and store that into day
        if (day.DayOfWeek != DayOfWeek.Monday) day = day.AddDays(-((int)day.DayOfWeek - 1));

        var weekly = context.WeeklyChallenges.AsNoTracking().FirstOrDefault(x => x.StartDate == day);

        if (weekly == null)
        {
            var generator = new WeeklyChallengeGenerator(day);

            weekly = await generator.Generate(context);
        }

        return Ok(new
        {
            Week = weekly.Week,
            StartDate = weekly.StartDate,
            EndDate = weekly.StartDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(7).AddSeconds(-1),
            Mods = weekly.Mods,
            Condtions = weekly.Conditions.OrderBy(x => x.Type).Select(x => new { x.Type, x.Value }).ToHashSet()
        });
    }

    [HttpGet]
    [Route("{username}/progression")]
    public async Task<IActionResult> GetWeeklyProgression(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("No username provided");

        username = username.ToLower();

        var user = context.Users.AsNoTracking().FirstOrDefault(x => x.Username == username);

        if (user == null)
            return NotFound($"User '{username}' not found");

        var day = DateOnly.FromDateTime(DateTime.Now);
        var week = (byte) ISOWeek.GetWeekOfYear(day.ToDateTime(TimeOnly.MinValue));

        var weeklyProgression = await context.WeeklyProgresses.AsNoTracking()
            .Include(weeklyProgress => weeklyProgress.ConditionProgresses)
            .ThenInclude(weeklyConditionProgress => weeklyConditionProgress.WeeklyChallengeCondition)
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.WeeklyChallenge.Week == week);

        if(weeklyProgression == null)
            return NotFound($"No progression found for user '{username}' for week {week}");

        return Ok(new
        {
            Week = week,
            Progress = weeklyProgression.ConditionProgresses.Select(x => new { x.WeeklyChallengeCondition.Type, x.CurrentProgress, x.IsCompleted }).OrderBy(x => x.Type)
        });
    }
}