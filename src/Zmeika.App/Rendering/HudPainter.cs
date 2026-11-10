using System.Globalization;

namespace Zmeika.App.Rendering;

internal static class HudPainter
{
    private const int TextX = ScreenLayout.HudTextX;
    private const int KeyX = TextX + 8;
    private const int TextWidth = ScreenLayout.HudWidth - 5;
    private const int KeyWidth = TextWidth - (KeyX - TextX);

    public static void Paint(FrameBuffer buffer, HudModel model, HudTheme theme)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(theme);

        FrameDrawing.Draw(buffer, ScreenLayout.HudX, ScreenLayout.HudY, ScreenLayout.HudWidth, ScreenLayout.HudHeight, FrameStyle.Single, theme.Frame);

        buffer.Write(TextX, 2, Invariant($"LEVEL {model.Level} / {model.LevelCount}"), theme.Level);
        buffer.Write(TextX, 4, "Player", theme.Label);
        buffer.Write(TextX, 5, Fit(model.Player, TextWidth), theme.PlayerName);
        buffer.Put(TextX, 7, Glyphs.Heart, theme.Heart);
        buffer.Write(TextX + 2, 7, Invariant($"{model.Lives}"), theme.Value);
        buffer.Put(TextX, 9, Glyphs.Apple, theme.Apple);
        buffer.Write(TextX + 2, 9, Invariant($"{model.ApplesEaten} / {model.ApplesRequired}"), theme.Value);
        buffer.Write(TextX, 11, "Score", theme.Label);
        buffer.Write(TextX, 12, Invariant($"{model.Score}"), theme.Score);

        HudControls controls = model.Controls;
        buffer.Write(TextX, 22, "Controls", theme.ControlsCaption);
        WriteControl(buffer, 24, "Up", controls.Up, theme);
        WriteControl(buffer, 25, "Down", controls.Down, theme);
        WriteControl(buffer, 26, "Left", controls.Left, theme);
        WriteControl(buffer, 27, "Right", controls.Right, theme);
        WriteControl(buffer, 29, "Pause", controls.Pause, theme);
    }

    private static void WriteControl(FrameBuffer buffer, int row, string direction, string key, HudTheme theme)
    {
        buffer.Write(TextX, row, direction, theme.ControlDirection);
        buffer.Write(KeyX, row, Fit(key, KeyWidth), theme.ControlKey);
    }

    private static string Fit(string text, int width) => text.Length <= width ? text : text[..width];

    private static string Invariant(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);
}
