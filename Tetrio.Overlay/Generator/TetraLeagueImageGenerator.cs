using SkiaSharp;
using TetraLeague.Overlay.Network.Api.Models;

namespace TetraLeague.Overlay.Generator;

public class TetraLeagueImageGenerator : BaseImageGenerator
{
    /// <summary>
    /// Generates an image containing the statistics for a specified user.
    /// </summary>
    /// <param name="user">The user of the player whose statistics are to be displayed.</param>
    /// <param name="stats">An object containing the player's TetraLeague statistics.</param>
    /// <param name="textColor">Optional. The color of the text in the image. Defaults to null.</param>
    /// <param name="backgroundColor">Optional. The background color of the image. Defaults to null.</param>
    /// <param name="displayUsername"></param>
    /// <returns>A memory stream containing the generated image with the player's statistics.</returns>
    public async Task<MemoryStream> GenerateTetraLeagueImage(TetrioUser user, Network.Api.Models.TetraLeague stats, string? textColor = null, string? backgroundColor = null, bool displayUsername = true)
    {
        var width = 900;
        var height = 300;
        var stream = new MemoryStream();
        var typeFace = SKTypeface.FromFile("wwwroot/web/res/fonts/cr.ttf");

        var placements = stats.Gamesplayed < 10;
        var unranked = placements || stats.Rank == "z";

        #region Paints

        textColor ??= "FFFFFF";

        var bigTextPaint = new SKPaint
        {
            Color = SKColor.Parse(textColor),
            Style = SKPaintStyle.Fill,
            TextSize = 64,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace
        };

        var bigTextShadowPaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            TextSize = 64,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace
        };

        var normalTextPaint = new SKPaint
        {
            Color = SKColor.Parse(textColor),
            Style = SKPaintStyle.Fill,
            TextSize = 32,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Left,
        };

        var normalTextShadowPaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            TextSize = 32,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Left,
        };

        var smallTextPaint = new SKPaint
        {
            Color = SKColor.Parse(textColor),
            Style = SKPaintStyle.Fill,
            TextSize = 20,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Right,
        };

        var smallTextShadowPaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            TextSize = 20,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Right,
        };

        var statsPaintAlt = new SKPaint
        {
            Color = SKColor.Parse(textColor),
            Style = SKPaintStyle.Fill,
            TextSize = 32,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace
        };

        var progressBarBg = new SKPaint
        {
            Color = SKColor.Parse("DD000000"),
            Style = SKPaintStyle.Fill,
            TextSize = 32,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace
        };

        var progressBarBgAlt = new SKPaint
        {
            Color = SKColor.Parse($"FF{textColor}"),
            Style = SKPaintStyle.Fill,
            TextSize = 32,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace
        };

        #endregion

        using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul));

        SetBackground(surface, width, height, backgroundColor);

        var rankBitmap = unranked ? GetBitmap("wwwroot/web/res/img/z.png") : GetBitmap($"wwwroot/web/res/img/{stats.Rank}.png");
        var toprankBitmap = placements ? GetBitmap("wwwroot/web/res/img/z.png") : GetBitmap($"wwwroot/web/res/img/{stats.TopRank}.png");

        var prevRank = stats.PrevRank == null ? null : GetBitmap($"wwwroot/web/res/img/{stats.PrevRank}.png");
        var nextRank = stats.NextRank == null ? GetBitmap("wwwroot/web/res/img/leaderboard1.png") : GetBitmap($"wwwroot/web/res/img/{stats.NextRank}.png");

        // Big Rank Letter
        surface.Canvas.DrawBitmap(ResizeBitmap(rankBitmap, 200, 200), 40, 40);

        // Username
        if(displayUsername)
            DrawTextWithShadow(surface, user.Username.ToUpper(), 275, 75, bigTextPaint, bigTextShadowPaint);

        // TR
        if(!placements)
            DrawTextWithShadow(surface, $"{stats.Tr:#.##} TR", 275, 140, bigTextPaint, bigTextShadowPaint);
        else
            DrawTextWithShadow(surface, $"{stats.Gamesplayed} / 10 placements", 275, 140, bigTextPaint, bigTextShadowPaint);

        // Stats
        surface.Canvas.DrawRect(462, 162, 3, 85, normalTextPaint);

        // Left Side
        var apm = stats.Apm ?? 0;
        var pps = stats.Pps ?? 0;
        var vs = stats.Vs ?? 0;

        if (stats.Gamesplayed > 0)
        {
            DrawTextWithShadow(surface, $"{apm}", 275, 185, normalTextPaint, normalTextShadowPaint); DrawTextWithShadow(surface, "APM", 385, 185, statsPaintAlt, normalTextShadowPaint);
            DrawTextWithShadow(surface, $"{pps}", 275, 215, normalTextPaint, normalTextShadowPaint); DrawTextWithShadow(surface, "PPS", 385, 215, statsPaintAlt, normalTextShadowPaint);
            DrawTextWithShadow(surface, $"{vs}", 275, 245, normalTextPaint, normalTextShadowPaint); DrawTextWithShadow(surface, "VS", 385, 245, statsPaintAlt, normalTextShadowPaint);

            // Right Side
            var globalRank = stats.StandingGlobal == -1 ? "UNRANKED" : $"# {stats.StandingGlobal!.Value}";
            var localRank = unranked ? "UNRANKED" : stats.StandingLocal!.Value == -1 ? "HIDDEN" : $"# {stats.StandingLocal!.Value}";
            DrawTextWithShadow(surface, "GLOBAL", 475, 185, normalTextPaint, normalTextShadowPaint); DrawTextWithShadow(surface, globalRank, 630, 185, normalTextPaint, normalTextShadowPaint);
            DrawTextWithShadow(surface, "COUNTRY", 475, 215, normalTextPaint, normalTextShadowPaint); DrawTextWithShadow(surface, localRank, 630, 215, normalTextPaint, normalTextShadowPaint);

            if (!string.IsNullOrWhiteSpace(user.Country) && stats.StandingLocal!.Value > 0)
            {
                using (var client = new HttpClient())
                {
                    var countryData = await client.GetByteArrayAsync($"https://tetr.io/res/flags/{user.Country.ToLower()}.png");

                    var bitmap = ResizeBitmap(DecodeImage(countryData), 36, 22);

                    surface.Canvas.DrawBitmap(bitmap, 680 + (stats.StandingLocal.ToString()!.Length - 1) * 25, 195);
                }
            }

            if (!placements)
            {
                DrawTextWithShadow(surface, "TOP RANK", 475, 245, normalTextPaint, normalTextShadowPaint);
                surface.Canvas.DrawBitmap(ResizeBitmap(toprankBitmap, 32, 32), 630, 218);
            }
        }

        // Progressbar
        if (stats.PrevAt.HasValue && stats.NextAt.HasValue && stats.StandingGlobal.HasValue && !unranked)
        {
            double range = stats.PrevAt.Value - stats.NextAt.Value;
            double distance = stats.PrevAt.Value - stats.StandingGlobal.Value;
            double rankPercentage = distance / range * 100;

            var highlight = (int)(rankPercentage / 100 * (width - 200));

            if (highlight < 0) highlight = 0;

            highlight = highlight > width-200 ? width - 200 : highlight;

            surface.Canvas.DrawRect(100, 275, width - 200, 10, progressBarBg);
            surface.Canvas.DrawRect(100, 275, highlight, 10, progressBarBgAlt);

            if(stats.PrevRank != null && prevRank != null)
                surface.Canvas.DrawBitmap(ResizeBitmap(prevRank, 32, 32), 60, 265);

            if (stats.Rank == "d" && stats.PrevAt != -1)
                DrawTextWithShadow(surface, $"#{stats.PrevAt}", 90, 286, smallTextPaint, smallTextShadowPaint);

            if (stats.NextRank == null )
                surface.Canvas.DrawBitmap(ResizeBitmap(nextRank, 32, 32), width - 90, 262);
            else
                surface.Canvas.DrawBitmap(ResizeBitmap(nextRank, 32, 32), width - 90, 265);
        }

        using var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 80);

        data.SaveTo(stream);

        return stream;
    }
}