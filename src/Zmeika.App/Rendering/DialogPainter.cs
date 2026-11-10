namespace Zmeika.App.Rendering;

internal static class DialogPainter
{
    public static void Paint(FrameBuffer buffer, int x, int y, int width, int height, string title, MenuTheme theme)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(theme);

        for (var row = y + 1; row < y + height - 1; row++)
        {
            buffer.Write(x + 1, row, new string(' ', width - 2));
        }

        FrameDrawing.Draw(buffer, x, y, width, height, FrameStyle.Double, theme.Frame);
        buffer.Write(ScreenLayout.CenterX(x, width, title.Length), y + 1, title, theme.Title);
    }
}
