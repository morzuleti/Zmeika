using System.Collections.Frozen;
using Zmeika.App.Rendering;

namespace Zmeika.App.Preview.Pages;

internal sealed class PalettePage(Theme theme) : IPreviewPage
{
    private const int SwatchX = 3;
    private const int SwatchWidth = 6;
    private const int NameX = 11;
    private const int HexX = 23;
    private const int UsageX = 33;
    private const int FirstRow = 4;

    private static readonly FrozenDictionary<string, string> s_usage = new Dictionary<string, string>
    {
        ["Red"] = "level 1, red apple, heart, 1st place",
        ["Orange"] = "level 2, score",
        ["Yellow"] = "level 3, yellow apple, current player, selection",
        ["Green"] = "level 4, green apple",
        ["LightBlue"] = "level 5, player name, frames",
        ["Blue"] = "level 6",
        ["Violet"] = "level 7, Controls caption",
        ["Pink"] = "fireworks",
        ["Cyan"] = "snake colour option",
        ["White"] = "regular text, key names, snake (default)",
        ["Gray"] = "hints, labels, inactive items",
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    public string Title => "Base palette";

    public void Draw(FrameBuffer buffer)
    {
        Rgb label = theme.Menu.Hint;
        buffer.Write(NameX, 2, "Colour", label);
        buffer.Write(HexX, 2, "Hex", label);
        buffer.Write(UsageX, 2, "Used for", label);

        for (var i = 0; i < theme.Palette.Count; i++)
        {
            NamedColor color = theme.Palette[i];
            int row = FirstRow + (i * 2);
            buffer.FillBackground(SwatchX, row, SwatchWidth, 1, color.Color);
            buffer.Write(NameX, row, color.Name, color.Color);
            buffer.Write(HexX, row, color.Color.ToHex(), theme.Menu.Item);
            buffer.Write(UsageX, row, s_usage.GetValueOrDefault(color.Name, string.Empty), label);
        }

        buffer.Write(NameX, FirstRow + (theme.Palette.Count * 2), "Window background: console default (black)", label);
    }
}
