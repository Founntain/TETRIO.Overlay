namespace Tetrio.Zenith.DailyChallenge.Models;

public class ZenithSplitResult
{
    public string Mods { get; set; }
    public DateTime? DateAchieved { get; set; }
    public uint GoldSplitTime { get; set; }
    public double AverageSplitTime { get; set; }

    public ZenithSplitResult()
    {

    }

    public ZenithSplitResult(string mods, DateTime? dateAchieved, uint goldSplitTime, double? averageSplitTime) : this()
    {
        this.Mods = mods;
        this.DateAchieved = dateAchieved;
        this.GoldSplitTime = goldSplitTime;
        this.AverageSplitTime = averageSplitTime ?? 0;
    }

    public string ToAverageTimeString() => TimeSpan.FromMilliseconds(this.AverageSplitTime).ToString(@"mm\:ss\.fff");
    public string ToGoldTimeString() => TimeSpan.FromMilliseconds(this.GoldSplitTime).ToString(@"mm\:ss\.fff");

    public string ToDateAchievedString()
    {
        if (DateAchieved == null) return "a long time ago";

        var now = DateTime.UtcNow;
        var dt = DateAchieved.Value.Kind == DateTimeKind.Utc ? DateAchieved.Value : DateAchieved.Value.ToUniversalTime();
        var span = now - dt;

        if (span.TotalSeconds < 0) span = TimeSpan.Zero;

        if (span.TotalSeconds < 5) return "just now";
        if (span.TotalSeconds < 60) return $"{(int)span.TotalSeconds} seconds ago";

        if (span.TotalMinutes < 2) return "a minute ago";
        if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} minutes ago";

        if (span.TotalHours < 2) return "an hour ago";
        if (span.TotalHours < 24) return $"{(int)span.TotalHours} hours ago";

        if (span.TotalDays < 2) return "yesterday";
        if (span.TotalDays < 30) return $"{(int)span.TotalDays} days ago";

        var months = (int)(span.TotalDays / 30);
        if (months < 2) return "a month ago";
        if (months < 12) return $"{months} months ago";

        var years = (int)(span.TotalDays / 365);
        return years < 2 ? "a year ago" : $"{years} years ago";
    }
}