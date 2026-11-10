using Zmeika.Core.Gameplay;
using Zmeika.Core.Model;

namespace Zmeika.App.Rendering;

internal static class FieldPainter
{
    private static readonly TimeSpan s_blinkHalfPeriod = TimeSpan.FromMilliseconds(250);

    public static void Paint(FrameBuffer buffer, LevelSession session, FieldStyle style)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(style);

        const int X = ScreenLayout.FieldX;
        const int Y = ScreenLayout.FieldY;
        LevelColorScheme scheme = style.Scheme;

        DrawGrid(buffer, X, Y, LevelSession.Field.Columns, LevelSession.Field.Rows, scheme);

        Apple apple = session.Apple;
        Rgb appleColor = style.AppleColors[apple.Variant % style.AppleColors.Count];
        DrawCell(buffer, X, Y, apple.Cell.X, apple.Cell.Y, Glyphs.Apple, appleColor, scheme);

        if (session.Heart is { } heart && IsBlinkVisible(session.HeartBlinkElapsed))
        {
            DrawCell(buffer, X, Y, heart.X, heart.Y, Glyphs.Heart, style.Heart, scheme);
        }

        IReadOnlyList<Position> segments = session.Snake.Segments;
        for (var i = segments.Count - 1; i >= 0; i--)
        {
            char glyph = i == 0 ? style.Skin.Head : style.Skin.Body;
            DrawCell(buffer, X, Y, segments[i].X, segments[i].Y, glyph, style.SnakeColor, scheme);
        }
    }

    public static void DrawGrid(FrameBuffer buffer, int x, int y, int columns, int rows, LevelColorScheme scheme)
    {
        ArgumentNullException.ThrowIfNull(scheme);

        FrameDrawing.Draw(buffer, x, y, (columns * ScreenLayout.CellWidth) + 2, rows + 2, FrameStyle.Double, scheme.Wall);
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                DrawCell(buffer, x, y, column, row, ' ', null, scheme);
            }
        }
    }

    public static void DrawCell(
        FrameBuffer buffer,
        int fieldX,
        int fieldY,
        int column,
        int row,
        char glyph,
        Rgb? color,
        LevelColorScheme scheme)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(scheme);

        Rgb background = scheme.ShadeAt(column, row);
        int x = fieldX + 1 + (column * ScreenLayout.CellWidth);
        int y = fieldY + 1 + row;
        buffer.Put(x, y, glyph, color, background);
        buffer.Put(x + 1, y, ' ', null, background);
    }

    private static bool IsBlinkVisible(TimeSpan? blinkElapsed) =>
        blinkElapsed is not { } elapsed || (long)(elapsed / s_blinkHalfPeriod) % 2 == 0;
}
