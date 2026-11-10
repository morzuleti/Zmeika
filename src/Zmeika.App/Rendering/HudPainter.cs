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

        int top = ScreenLayout.HudY;
        buffer.Write(TextX, top + 1, Invariant($"LEVEL {model.Level} / {model.LevelCount}"), theme.Level);
        buffer.Write(TextX, top + 3, "Player", theme.Label);
        buffer.Write(TextX, top + 4, Fit(model.Player, TextWidth), theme.PlayerName);
        buffer.Put(TextX, top + 6, Glyphs.Heart, theme.Heart);
        buffer.Write(TextX + 2, top + 6, Invariant($"{model.Lives}"), theme.Value);
        buffer.Put(TextX, top + 7, Glyphs.Apple, theme.Apple);
        buffer.Write(TextX + 2, top + 7, Invariant($"{model.ApplesEaten} / {model.ApplesRequired}"), theme.Value);
        buffer.Write(TextX, top + 9, "Score", theme.Label);
        buffer.Write(TextX, top + 10, Invariant($"{model.Score}"), theme.Score);

        HudControls controls = model.Controls;
        int bottom = ScreenLayout.HudY + ScreenLayout.HudHeight - 1;
        buffer.Write(TextX, bottom - 7, "Controls", theme.ControlsCaption);
        WriteControl(buffer, bottom - 6, "Up", controls.Up, theme);
        WriteControl(buffer, bottom - 5, "Down", controls.Down, theme);
        WriteControl(buffer, bottom - 4, "Left", controls.Left, theme);
        WriteControl(buffer, bottom - 3, "Right", controls.Right, theme);
        WriteControl(buffer, bottom - 2, "Pause", controls.Pause, theme);

        if (model.Warning is { } warning)
        {
            buffer.Write(TextX, bottom - 1, Fit(warning, TextWidth), theme.Label);
        }
    }

    private static void WriteControl(FrameBuffer buffer, int row, string direction, string key, HudTheme theme)
    {
        buffer.Write(TextX, row, direction, theme.ControlDirection);
        buffer.Write(KeyX, row, Fit(key, KeyWidth), theme.ControlKey);
    }

    private static string Fit(string text, int width) => text.Length <= width ? text : text[..width];

    private static string Invariant(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);
}
