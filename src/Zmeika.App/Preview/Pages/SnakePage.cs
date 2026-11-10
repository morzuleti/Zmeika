using System.Globalization;
using Zmeika.App.Rendering;

namespace Zmeika.App.Preview.Pages;

internal sealed class SnakePage(Theme theme) : IPreviewPage
{
    private const int LabelX = 2;
    private const int SamplesX = 16;
    private const int ColumnWidth = 10;
    private const int HeaderRow = 3;
    private const int FirstRow = 6;
    private const int RowSpacing = 4;

    public string Title => "Snake skins and colours (settings)";

    public void Draw(FrameBuffer buffer)
    {
        for (var c = 0; c < theme.SnakeColors.Count; c++)
        {
            NamedColor color = theme.SnakeColors[c];
            buffer.Write(SamplesX + (c * ColumnWidth), HeaderRow, color.Name, color.Color);
        }

        for (var s = 0; s < theme.Skins.Count; s++)
        {
            SnakeSkin skin = theme.Skins[s];
            int row = FirstRow + (s * RowSpacing);
            buffer.Write(LabelX, row, string.Create(CultureInfo.InvariantCulture, $"{s + 1} {skin.Name}"), theme.Menu.Item);
            buffer.Write(LabelX + 2, row + 1, $"{skin.Head} head  {skin.Body} body", theme.Menu.Hint);

            for (var c = 0; c < theme.SnakeColors.Count; c++)
            {
                DrawSample(buffer, SamplesX + (c * ColumnWidth), row, skin, theme.SnakeColors[c].Color);
            }
        }

        int notesRow = FirstRow + (theme.Skins.Count * RowSpacing);
        buffer.Write(LabelX, notesRow, $"Default: {theme.DefaultSkin.Name}, {theme.DefaultSnakeColor.Name}.", theme.Menu.Title);
        buffer.Write(LabelX, notesRow + 1, "Green and Red are not offered: they would merge with apples and the heart.", theme.Menu.Hint);
        buffer.Write(LabelX, notesRow + 2, "Head and body use the same colour. Each cell is 2 characters wide.", theme.Menu.Hint);
    }

    private static void DrawSample(FrameBuffer buffer, int x, int y, SnakeSkin skin, Rgb color)
    {
        const int BodyLength = 3;
        for (var i = 0; i < BodyLength; i++)
        {
            buffer.Put(x + (i * ScreenLayout.CellWidth), y, skin.Body, color);
        }

        buffer.Put(x + (BodyLength * ScreenLayout.CellWidth), y, skin.Head, color);
    }
}
