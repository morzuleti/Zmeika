using System.Globalization;
using Zmeika.App.Input;
using Zmeika.App.Rendering;

namespace Zmeika.App.Preview.Pages;

internal sealed class LevelScreenPage(Theme theme) : IPreviewPage
{
    private const int SampleApplesPerLevel = 10;

    private int _levelIndex;

    public string Title => string.Create(CultureInfo.InvariantCulture, $"Level screen, level {_levelIndex + 1}");

    public bool IsFullScreen => true;

    public string KeyHint => string.Create(CultureInfo.InvariantCulture, $"1-{theme.Levels.Count} level colour");

    public void Draw(FrameBuffer buffer)
    {
        LevelColorScheme scheme = theme.Levels[_levelIndex];
        SnakeSkin skin = theme.DefaultSkin;
        Rgb snakeColor = theme.DefaultSnakeColor.Color;

        FieldPainter.DrawGrid(buffer, ScreenLayout.FieldX, ScreenLayout.FieldY, ScreenLayout.FieldColumns, ScreenLayout.FieldRows, scheme);
        DrawFieldObject(buffer, 29, 26, skin.Head, snakeColor, scheme);
        DrawFieldObject(buffer, 29, 27, skin.Body, snakeColor, scheme);
        DrawFieldObject(buffer, 29, 28, skin.Body, snakeColor, scheme);
        DrawFieldObject(buffer, 29, 29, skin.Body, snakeColor, scheme);
        DrawFieldObject(buffer, 6, 4, Glyphs.Apple, theme.AppleColors[^1], scheme);
        DrawFieldObject(buffer, 17, 12, Glyphs.Heart, theme.Heart, scheme);

        DrawHud(buffer);
    }

    public bool HandleKey(ConsoleKeyInfo key)
    {
        int level = key.KeyChar - '0';
        if (level >= 1 && level <= theme.Levels.Count)
        {
            _levelIndex = level - 1;
            return true;
        }

        return false;
    }

    private static void DrawFieldObject(FrameBuffer buffer, int column, int row, char glyph, Rgb color, LevelColorScheme scheme) =>
        FieldPainter.DrawCell(buffer, ScreenLayout.FieldX, ScreenLayout.FieldY, column, row, glyph, color, scheme);

    private void DrawHud(FrameBuffer buffer)
    {
        int level = _levelIndex + 1;
        HudModel sample = new(
            level,
            theme.Levels.Count,
            "Masha",
            Lives: 3,
            ApplesEaten: 4,
            ApplesRequired: level * SampleApplesPerLevel,
            Score: 200,
            KeyNames.ForHud(KeyBindings.Default));
        HudPainter.Paint(buffer, sample, theme.Hud);
    }
}
