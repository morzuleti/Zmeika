using System.Globalization;
using Zmeika.App.Rendering;

namespace Zmeika.App.Preview.Pages;

internal sealed class LevelColorsPage(Theme theme) : IPreviewPage
{
    private const int SampleColumns = 8;
    private const int SampleRows = 3;
    private const int BlockHeight = 6;
    private const int FirstRow = 2;
    private const int LeftColumnX = 2;
    private const int RightColumnX = 46;
    private const int LabelOffsetX = 20;
    private const int BlocksPerColumn = 4;

    private static readonly (int Column, int Row)[] s_appleCells = [(6, 0), (7, 2), (6, 2)];

    public string Title => "Level colours: wall, grid shades, objects on top";

    public void Draw(FrameBuffer buffer)
    {
        for (var i = 0; i < theme.Levels.Count; i++)
        {
            int x = i < BlocksPerColumn ? LeftColumnX : RightColumnX;
            int y = FirstRow + ((i % BlocksPerColumn) * BlockHeight);
            DrawBlock(buffer, x, y, theme.Levels[i]);
        }
    }

    private void DrawBlock(FrameBuffer buffer, int x, int y, LevelColorScheme scheme)
    {
        FieldSketch.DrawField(buffer, x, y, SampleColumns, SampleRows, scheme);
        FieldSketch.DrawHorizontalSnake(buffer, x, y, 1, 1, theme.DefaultSkin, theme.DefaultSnakeColor.Color, scheme);
        int appleCount = Math.Min(theme.AppleColors.Count, s_appleCells.Length);
        for (var i = 0; i < appleCount; i++)
        {
            (int column, int row) = s_appleCells[i];
            FieldSketch.DrawCell(buffer, x, y, column, row, Glyphs.Apple, theme.AppleColors[i], scheme);
        }

        FieldSketch.DrawCell(buffer, x, y, 0, 2, Glyphs.Heart, theme.Heart, scheme);

        int labelX = x + LabelOffsetX;
        buffer.Write(labelX, y, string.Create(CultureInfo.InvariantCulture, $"Level {scheme.Level}: {scheme.Name}"), scheme.Wall);
        WriteSwatch(buffer, labelX, y + 1, "Wall   ", scheme.Wall);
        WriteSwatch(buffer, labelX, y + 2, "Shade A", scheme.ShadeA);
        WriteSwatch(buffer, labelX, y + 3, "Shade B", scheme.ShadeB);
    }

    private void WriteSwatch(FrameBuffer buffer, int x, int y, string label, Rgb color)
    {
        buffer.Write(x, y, label, theme.Menu.Hint);
        buffer.FillBackground(x + 8, y, 3, 1, color);
        buffer.Write(x + 12, y, color.ToHex(), theme.Menu.Item);
    }
}
