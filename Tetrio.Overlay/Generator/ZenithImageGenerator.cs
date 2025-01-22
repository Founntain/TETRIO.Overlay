using SkiaSharp;
using TetraLeague.Overlay.Network.Api.Models;

namespace TetraLeague.Overlay.Generator;

public class ZenithImageGenerator : BaseImageGenerator
{
    public MemoryStream GenerateZenithImage(string username, QuickPlay stats, QuickPlay? expert, string? textColor, string? backgroundColor, bool displayUsername)
    {
        var width = 900;
        var height = 300;
        var center = (float) width / 2;
        var stream = new MemoryStream();
        var typeFace = SKTypeface.FromFile("wwwroot/web/res/fonts/cr.ttf");

        #region Paints

        textColor ??= "FFFFFF";

        var bigTextPaint = new SKPaint
        {
            Color = SKColor.Parse(textColor),
            Style = SKPaintStyle.Fill,
            TextSize = 64,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        var bigTextShadowPaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            TextSize = 64,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        var normalTextPaint = new SKPaint
        {
            Color = SKColor.Parse(textColor),
            Style = SKPaintStyle.Fill,
            TextSize = 32,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        var normalTextShadowPaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            TextSize = 32,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        var smallTextPaint = new SKPaint
        {
            Color = SKColor.Parse(textColor),
            Style = SKPaintStyle.Fill,
            TextSize = 20,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        var smallTextShadowPaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            TextSize = 20,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        #endregion

        using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul));

        SetBackground(surface, width, height, backgroundColor);

        var offset = 0;

        // USERNAME
        if (displayUsername)
        {
            DrawTextWithShadow(surface, username.ToUpper(), center, 55, bigTextPaint, bigTextShadowPaint);
        }
        else
        {
            DrawTextWithShadow(surface, "QUICK PLAY", center, 55, bigTextPaint, bigTextShadowPaint);
        }

        offset += 65;

        var normalCenterValue = expert?.Record == null ? center : center / 2;

        // NORMAL
        var currentWeekPps = $"{stats.Record!.Results.Aggregatestats.Pps:F2} PPS";
        var currentWeekApm = $"{stats.Record.Results.Aggregatestats.Apm:F2} APM";
        var currentWeekVs = $"{stats.Record.Results.Aggregatestats.Vsscore:F2} VS";

        // We only draw the NORMAL text if we have an expert record as well
        if(expert?.Record != null)
            DrawTextWithShadow(surface, "NORMAL", normalCenterValue, 15 + offset, smallTextPaint, smallTextShadowPaint);

        DrawTextWithShadow(surface, $"{stats.Record!.Results.Stats.Zenith.Altitude:F2} m", normalCenterValue, 65 + offset, bigTextPaint, bigTextShadowPaint);
        DrawTextWithShadow(surface, $"{currentWeekPps} | {currentWeekApm} | {currentWeekVs}", normalCenterValue, 88 + offset, smallTextPaint, smallTextShadowPaint);

        // We only draw the personal best when it exists AND is better than the current week
        if (stats.Best?.Record != null && stats.Best.Record.Results.Stats.Zenith.Altitude != stats.Record.Results.Stats.Zenith.Altitude)
        {
            DrawTextWithShadow(surface, $"PB {stats.Best.Record.Results.Stats.Zenith.Altitude:F2} m", normalCenterValue, 120 + offset, normalTextPaint, normalTextShadowPaint);
        }
        else
        {
            offset -= 30;
        }

        var modsCurrentWeek = stats.Record.Extras.Zenith.Mods;
        // modsCurrentWeek = new []{"allspin", "allspin", "allspin", "allspin", "allspin", "allspin", "allspin", "allspin", "allspin", "allspin"};

        var modSize = 48;
        var modCanvasWidthCurrentWeek = (modSize * modsCurrentWeek.Length) + (10 * (modsCurrentWeek.Length - 1));

        if (modsCurrentWeek.Length > 0)
        {
            var currentWeekModsImage = GenerateModImage(ref modCanvasWidthCurrentWeek, modSize, modsCurrentWeek);

            surface.Canvas.DrawImage(currentWeekModsImage, normalCenterValue - modCanvasWidthCurrentWeek / 2, 130 + offset);
        }

        // EXPERT
        if (stats.Best?.Record == null || stats.Best.Record.Results.Stats.Zenith.Altitude == stats.Record.Results.Stats.Zenith.Altitude)
        {
            offset += 30;
        }

        if(expert?.Record != null)
        {
            var modsExpert = expert.Record.Extras.Zenith.Mods;
            int modCanvasWidthPersonalBest = (modSize * modsExpert.Length) + (10 * (modsExpert.Length - 1));

            var expertPps = $"{expert.Record.Results.Aggregatestats.Pps:F2} PPS";
            var expertApm = $"{expert.Record.Results.Aggregatestats.Apm:F2} APM";
            var expertVs = $"{expert.Record.Results.Aggregatestats.Vsscore:F2} VS";

            DrawTextWithShadow(surface, "EXPERT", (normalCenterValue) * 3, 15 + offset, smallTextPaint, smallTextShadowPaint);
            DrawTextWithShadow(surface, $"{expert.Record.Results.Stats.Zenith.Altitude:F2} m", (normalCenterValue) * 3, 65 + offset, bigTextPaint, bigTextShadowPaint);
            DrawTextWithShadow(surface, $"{expertPps} | {expertApm} | {expertVs}", (normalCenterValue) * 3, 88 + offset, smallTextPaint, smallTextShadowPaint);

            // We only draw the personal best when it exists AND is better than the current week
            if (expert.Best?.Record != null && expert.Best.Record.Results.Stats.Zenith.Altitude != expert.Record.Results.Stats.Zenith.Altitude)
            {
                DrawTextWithShadow(surface, $"PB {expert.Best.Record.Results.Stats.Zenith.Altitude:F2} m", (normalCenterValue) * 3, 120 + offset, normalTextPaint, normalTextShadowPaint);
            }
            else
            {
                offset -= 30;
            }

            if (modsExpert.Length > 0)
            {
                var personalBestModsImage = GenerateModImage(ref modCanvasWidthPersonalBest, modSize, modsExpert);

                surface.Canvas.DrawImage(personalBestModsImage, 3 * (center / 2) - modCanvasWidthPersonalBest / 2, 130 + offset);
            }
        }

        using var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 80);

        data.SaveTo(stream);

        return stream;
    }

    public MemoryStream GenerateZenithSplitsImage(string username, ZenithRecords stats, QuickPlay careerBest, string? textColor, string? backgroundColor, bool displayUsername)
    {
        var width = 1500;
        var height = 200;
        var center = (float) width / 2;
        var stream = new MemoryStream();
        var typeFace = SKTypeface.FromFile("wwwroot/web/res/fonts/cr.ttf");

        #region Paints

        textColor ??= "FFFFFF";

        var bigTextPaint = new SKPaint
        {
            Color = SKColor.Parse(textColor),
            Style = SKPaintStyle.Fill,
            TextSize = 64,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        var bigTextShadowPaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            TextSize = 64,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        var normalTextPaint = new SKPaint
        {
            Color = SKColor.Parse(textColor),
            Style = SKPaintStyle.Fill,
            TextSize = 32,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        var normalTextShadowPaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            TextSize = 32,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        var smallTextPaint = new SKPaint
        {
            Color = SKColor.Parse(textColor),
            Style = SKPaintStyle.Fill,
            TextSize = 20,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        var smallTextShadowPaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            TextSize = 20,
            FakeBoldText = true,
            IsAntialias = true,
            Typeface = typeFace,
            TextAlign = SKTextAlign.Center,
        };

        #endregion

        if (!displayUsername)
            height -= 65;

        using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul));

        SetBackground(surface, width, height, backgroundColor);

        var offset = 0;

        // TITLE
        if (displayUsername)
        {
            DrawTextWithShadow(surface, $"{username.ToUpper()}'S SPLITS", center, 65, bigTextPaint, bigTextShadowPaint);

            offset = 65;
        }

        var goldSplits = new int[9];

        foreach (var entry in stats.Entries)
        {
            List<double?> splits = entry.Results.Stats.Zenith.Splits;

            for (var i = 0; i < splits.Count; i++)
            {
                var split = splits[i];

                if(split == null) continue;

                if (goldSplits[i] == 0)
                {

                    goldSplits[i] = (int) split;

                    continue;
                }

                if (goldSplits[i] > split && split != 0)
                {
                    goldSplits[i] = (int) split;
                }
            }
        }

        var recentSplits = stats.Entries.First().Results.Stats.Zenith.Splits.Select(x => (int) (x ?? 0)).ToArray();
        var careerBestSplits = careerBest.Best!.Record!.Results.Stats.Zenith.Splits.Select(x => (int) (x ?? 0)).ToArray();

        var splitsImageWidth = 0;

        var splitsImage = GenerateSplitsImage(ref splitsImageWidth, goldSplits, recentSplits, careerBestSplits, normalTextPaint, smallTextPaint, normalTextShadowPaint, smallTextShadowPaint);

        surface.Canvas.DrawImage(splitsImage, center - ( splitsImageWidth / 2 ), 25 + offset);

        using var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 80);

        data.SaveTo(stream);

        return stream;
    }

    private SKImage GenerateModImage(ref int modCanvasWidth, int modSize, string[] mods)
    {
        var height = mods.Length > 4 ? modSize * 2 : modSize;
        var factor = mods.Length > 4 ? 4 : mods.Length;

        modCanvasWidth = (modSize * factor) + (10 * (factor - 1));

        using var modSurface = SKSurface.Create(new SKImageInfo(modCanvasWidth, height + 5, SKColorType.Bgra8888, SKAlphaType.Unpremul));

        for (var i = 0; i < mods.Length; i++)
        {
            var mod = mods[i];
            var modBitmap = ResizeBitmap(GetBitmap($"wwwroot/web/res/img/{mod}.png"), modSize, modSize);

            if (i >= 4)
            {
                var subImageWidth = modSize * (mod.Length - i) + (10 * (mod.Length - i - 1));
                var image = GenerateModImage(ref subImageWidth, modSize, mods.Skip(i).ToArray());

                modSurface.Canvas.DrawImage(image, modCanvasWidth / 2 - subImageWidth / 2, modSize + 5);

                break;
            }

            var offset = modSize * i + 10 * i;

            modSurface.Canvas.DrawBitmap(modBitmap, offset, 0);
        }

        var modImage = modSurface.Snapshot();

        return modImage;
    }

    private SKImage GenerateSplitsImage(ref int splitsImageWidth, int[] goldSplits, int[] recentSplits, int[] careerBestSplits, SKPaint textPaint, SKPaint smallTextPain, SKPaint shawdowPaint, SKPaint smallShadowPaint)
    {
        var height = 85;
        var rectWidth = 165;
        var rectWidthHalf = rectWidth / 2;

        var floorColors = new[]
        {
            "AAfde692",
            "AAffc788",
            "AAffb7c2",
            "AAffba43",
            "AAff917b",
            "AA00ddff",
            "AAff006f",
            "AA98ffb2",
            "AAd677ff",
        };

        var floorNames = new[]
        {
            "HOTEL",
            "CASINO",
            "ARENA",
            "MUSEUM",
            "OFFICES",
            "LABORATORY",
            "THE CORE",
            "CORRUPTION",
            "POTG",
        };

        var amountOfSplits = 0;

        if (goldSplits.All(x => x > 0))
        {
            amountOfSplits = goldSplits.Length;
        }
        else
        {
            for (var i = 0; i < goldSplits.Length; i++)
            {
                if (goldSplits[i] == 0)
                {
                    amountOfSplits = i + 1;

                    break;
                }
            }
        }

        splitsImageWidth = (rectWidth * amountOfSplits) - 6;

        using var surface = SKSurface.Create(new SKImageInfo(splitsImageWidth, height, SKColorType.Bgra8888, SKAlphaType.Unpremul));

        bool notReached = false;

        for (var i = 0; i < goldSplits.Length; i++)
        {
            var split = goldSplits[i];

            var backgroundPaint = new SKPaint
            {
                Color = SKColor.Parse(floorColors[i]),
                Style = SKPaintStyle.Fill,
                FakeBoldText = true,
                IsAntialias = true,
            };

            var blackOverlayPaint = new SKPaint
            {
                Color = SKColor.Parse("AA000000"),
                Style = SKPaintStyle.Fill,
                FakeBoldText = true,
                IsAntialias = true,
            };

            var time = TimeSpan.FromMilliseconds(split);

            var isRecentSplitsEmpty = recentSplits.All(x => x == 0);

            var recentTime = recentSplits[i] == 0 ? TimeSpan.FromMilliseconds(0) : TimeSpan.FromMilliseconds(recentSplits[i] - split);

            if (isRecentSplitsEmpty)
            {
                if (careerBestSplits.Any(x => x != 0))
                {
                    isRecentSplitsEmpty = false;
                }

                recentTime = TimeSpan.FromMilliseconds(careerBestSplits[i] - split);
            }

            var isSlower = recentTime.TotalMilliseconds > 0;
            var prefix = isSlower ? "+" : "-";

            if (split == 0)
            {
                surface.Canvas.DrawRect(0 + (i * rectWidth), 0, rectWidth - 6, 85, backgroundPaint);
                surface.Canvas.DrawRect(3 + (i * rectWidth), 3, rectWidth - 12, 79, blackOverlayPaint);

                DrawTextWithShadow(surface, $"{floorNames[i]}", rectWidthHalf + (i * rectWidth), 25, smallTextPain, smallShadowPaint);
                DrawTextWithShadow(surface, "DNF", rectWidthHalf + (i * rectWidth), 55, textPaint, shawdowPaint);

                break;
            }

            surface.Canvas.DrawRect(0 + (i * rectWidth), 0, rectWidth - 6, 85, backgroundPaint);
            surface.Canvas.DrawRect(3 + (i * rectWidth), 3, rectWidth - 12, 79, blackOverlayPaint);

            DrawTextWithShadow(surface, $"{floorNames[i]}", rectWidthHalf + (i * rectWidth), 25, smallTextPain, smallShadowPaint);
            DrawTextWithShadow(surface, time.ToString(@"m\:ss\.fff"), rectWidthHalf + (i * rectWidth), 55, textPaint, shawdowPaint);

            smallTextPain.Color = isSlower ? SKColor.Parse("EE005B") : SKColor.Parse("00EE00");

            if((recentTime.TotalMilliseconds != 0 || ( recentTime.TotalMilliseconds == 0 && split != 0 && recentSplits[i] != 0) && !isRecentSplitsEmpty))
                DrawTextWithShadow(surface, prefix + recentTime.ToString(@"s\.fff"), rectWidthHalf + (i * rectWidth), 75, smallTextPain, smallShadowPaint);
            else if(!notReached)
            {
                smallTextPain.Color = SKColors.DarkGray;

                DrawTextWithShadow(surface, "NOT REACHED", rectWidthHalf + (i * rectWidth), 75, smallTextPain, smallShadowPaint);

                notReached = true;
            }

            smallTextPain.Color = SKColor.Parse("FFFFFF");
        }


        var image = surface.Snapshot();

        return image;
    }
}