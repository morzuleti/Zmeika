using Zmeika.App.Rendering;

namespace Zmeika.App.Preview;

internal static class FieldSketch
{
    public static void DrawField(FrameBuffer buffer, int x, int y, int columns, int rows, LevelColorScheme scheme)
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

    public static void DrawHorizontalSnake(
        FrameBuffer buffer,
        int fieldX,
        int fieldY,
        int tailColumn,
        int row,
        SnakeSkin skin,
        Rgb color,
        LevelColorScheme scheme)
    {
        ArgumentNullException.ThrowIfNull(skin);

        const int BodyLength = 3;
        for (var i = 0; i < BodyLength; i++)
        {
            DrawCell(buffer, fieldX, fieldY, tailColumn + i, row, skin.Body, color, scheme);
        }

        DrawCell(buffer, fieldX, fieldY, tailColumn + BodyLength, row, skin.Head, color, scheme);
    }
}
