using System.Globalization;
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
        const int KeyX = ScreenLayout.HudTextX + 8;
        HudTheme hud = theme.Hud;
        int level = _levelIndex + 1;

        FrameDrawing.Draw(buffer, ScreenLayout.HudX, ScreenLayout.HudY, ScreenLayout.HudWidth, ScreenLayout.HudHeight, FrameStyle.Single, hud.Frame);

        buffer.Write(ScreenLayout.HudTextX, 2, string.Create(CultureInfo.InvariantCulture, $"LEVEL {level} / {theme.Levels.Count}"), hud.Level);
        buffer.Write(ScreenLayout.HudTextX, 4, "Player", hud.Label);
        buffer.Write(ScreenLayout.HudTextX, 5, "Masha", hud.PlayerName);
        buffer.Put(ScreenLayout.HudTextX, 7, Glyphs.Heart, hud.Heart);
        buffer.Write(ScreenLayout.HudTextX + 2, 7, "3", hud.Value);
        buffer.Put(ScreenLayout.HudTextX, 9, Glyphs.Apple, hud.Apple);
        buffer.Write(ScreenLayout.HudTextX + 2, 9, string.Create(CultureInfo.InvariantCulture, $"4 / {level * SampleApplesPerLevel}"), hud.Value);
        buffer.Write(ScreenLayout.HudTextX, 11, "Score", hud.Label);
        buffer.Write(ScreenLayout.HudTextX, 12, "200", hud.Score);

        buffer.Write(ScreenLayout.HudTextX, 22, "Controls", hud.ControlsCaption);
        WriteControl(buffer, 24, "Up", Glyphs.ArrowUp.ToString(), KeyX);
        WriteControl(buffer, 25, "Down", Glyphs.ArrowDown.ToString(), KeyX);
        WriteControl(buffer, 26, "Left", Glyphs.ArrowLeft.ToString(), KeyX);
        WriteControl(buffer, 27, "Right", Glyphs.ArrowRight.ToString(), KeyX);
        WriteControl(buffer, 29, "Pause", "Space", KeyX);
    }

    private void WriteControl(FrameBuffer buffer, int row, string direction, string key, int keyX)
    {
        buffer.Write(ScreenLayout.HudTextX, row, direction, theme.Hud.ControlDirection);
        buffer.Write(keyX, row, key, theme.Hud.ControlKey);
    }
}
