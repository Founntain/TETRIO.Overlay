using SkiaSharp;

namespace TetraLeague.Overlay.Generator;

public class BaseImageGenerator
{
    public MemoryStream GenerateUserNotFound()
    {
        var width = 900;
        var height = 300;
        var stream = new MemoryStream();
        var typeFace = SKTypeface.FromFile("wwwroot/web/res/fonts/cr.ttf");

        #region Paints

        var bigTextPaint = new SKPaint
        {
            Color = SKColor.Parse("5a6e49"),
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
            Color = SKColor.Parse("8bca95"),
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

        #endregion

        using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul));

        var errorBitmap = GetBitmap("wwwroot/web/res/img/error.png");

        SetBackground(surface, width, height, "0f160d");

        DrawTextWithShadow(surface, "No such user / record", width / 2, 60, bigTextPaint, bigTextShadowPaint);
        DrawTextWithShadow(surface, "Either you mistyped something", width / 2, 100, normalTextPaint, normalTextShadowPaint);
        DrawTextWithShadow(surface, "or the account no longer exists.", width / 2, 130, normalTextPaint, normalTextShadowPaint);

        surface.Canvas.DrawBitmap(errorBitmap, (width / 2) - (errorBitmap.Width / 2), 140);

        using var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 80);

        data.SaveTo(stream);

        return stream;
    }

    public MemoryStream GenerateErrorImage(string title, string? subText1 = null, string? subText2 = null)
    {
        var width = 900;
        var height = 300;
        var stream = new MemoryStream();
        var typeFace = SKTypeface.FromFile("wwwroot/web/res/fonts/cr.ttf");

        #region Paints

        var bigTextPaint = new SKPaint
        {
            Color = SKColor.Parse("5a6e49"),
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
            Color = SKColor.Parse("8bca95"),
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

        #endregion

        using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul));

        var errorBitmap = GetBitmap("wwwroot/web/res/img/error.png");

        SetBackground(surface, width, height, "0f160d");

        DrawTextWithShadow(surface, title, (float)width / 2, 60, bigTextPaint, bigTextShadowPaint);
        if (subText1 != null) DrawTextWithShadow(surface, subText1, (float)width / 2, 100, normalTextPaint, normalTextShadowPaint);
        if (subText2 != null) DrawTextWithShadow(surface, subText2, (float)width / 2, 130, normalTextPaint, normalTextShadowPaint);

        surface.Canvas.DrawBitmap(errorBitmap, (width / 2) - (errorBitmap.Width / 2), 140);

        using var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 80);

        data.SaveTo(stream);

        return stream;
    }

    public SKBitmap GetBitmap(string relativePath)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var fullPath = Path.Combine(currentDirectory, relativePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"The file {fullPath} does not exist.");
        }

        // Ensure the file is readable
        var fileInfo = new FileInfo(fullPath);

        if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
        {
            throw new UnauthorizedAccessException($"The file {fullPath} is not accessible for reading.");
        }

        using var stream = File.OpenRead(fullPath);
        using var managedStream = new SKManagedStream(stream);

        var bitmap = SKBitmap.Decode(managedStream);

        if (bitmap == null)
        {
            throw new ArgumentException($"Failed to decode the bitmap from {fullPath}");
        }

        return bitmap;
    }

    public SKBitmap ResizeBitmap(SKBitmap bitmap, int width, int height)
    {
        var resizedBitmap = new SKBitmap(width, height);

        using var canvas = new SKCanvas(resizedBitmap);
        using var paint = new SKPaint();

        paint.IsAntialias = true;
        paint.FilterQuality = SKFilterQuality.High;

        canvas.Clear(SKColors.Transparent);

        var destRect = new SKRect(0, 0, width, height);

        canvas.DrawBitmap(bitmap, destRect, paint);

        return resizedBitmap;
    }

    public void DrawTextWithShadow(SKSurface surface, string text, float x, float y, SKPaint textPaint, SKPaint shawdowPaint)
    {
        surface.Canvas.DrawText(text, x + 2, y + 2, shawdowPaint);
        surface.Canvas.DrawText(text, x + 3, y + 3, shawdowPaint);
        surface.Canvas.DrawText(text, x, y, textPaint);
    }

    public void SetBackground(SKSurface surface, float width, float height, string? backgroundColor)
    {
        if (backgroundColor == null) return;

        var backgroundColorPaint = new SKPaint
        {
            Color = SKColor.Parse(backgroundColor),
            Style = SKPaintStyle.Fill,
            TextSize = 32,
            FakeBoldText = true,
            IsAntialias = true,
        };

        surface.Canvas.DrawRect(0, 0, width, height, backgroundColorPaint);
    }

    public SKBitmap DecodeImage(byte[] imageData)
    {
        return SKBitmap.Decode(imageData);
    }
}