using Zmeika.App.Rendering;

namespace Zmeika.App.Preview;

internal static class FieldSketch
{
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
            FieldPainter.DrawCell(buffer, fieldX, fieldY, tailColumn + i, row, skin.Body, color, scheme);
        }

        FieldPainter.DrawCell(buffer, fieldX, fieldY, tailColumn + BodyLength, row, skin.Head, color, scheme);
    }
}
