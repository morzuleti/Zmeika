namespace Zmeika.App.Rendering;

internal static class FrameDrawing
{
    public static void Draw(FrameBuffer buffer, int x, int y, int width, int height, FrameStyle style, Rgb color)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(style);

        int right = x + width - 1;
        int bottom = y + height - 1;

        buffer.Put(x, y, style.TopLeft, color);
        buffer.Put(right, y, style.TopRight, color);
        buffer.Put(x, bottom, style.BottomLeft, color);
        buffer.Put(right, bottom, style.BottomRight, color);

        for (var column = x + 1; column < right; column++)
        {
            buffer.Put(column, y, style.Horizontal, color);
            buffer.Put(column, bottom, style.Horizontal, color);
        }

        for (var row = y + 1; row < bottom; row++)
        {
            buffer.Put(x, row, style.Vertical, color);
            buffer.Put(right, row, style.Vertical, color);
        }
    }
}
