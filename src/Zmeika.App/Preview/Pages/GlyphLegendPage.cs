using Zmeika.App.Rendering;

namespace Zmeika.App.Preview.Pages;

internal sealed class GlyphLegendPage : IPreviewPage
{
    private const int GlyphX = 3;
    private const int NameX = 17;
    private const int ColorX = 37;
    private const int PurposeX = 50;
    private const int FirstRow = 4;

    private readonly Theme _theme;
    private readonly IReadOnlyList<GlyphLegendEntry> _entries;

    public GlyphLegendPage(Theme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        _theme = theme;
        Rgb snake = theme.DefaultSnakeColor.Color;
        Rgb burst = theme.Fireworks.LevelBurstColors[0];
        _entries =
        [
            Named($"{theme.DefaultSkin.Head}", snake, "Snake head", "default skin, setting"),
            Named($"{theme.DefaultSkin.Body}", snake, "Snake body", "one body segment"),
            .. theme.AppleColors.Select((color, index) =>
                Named($"{Glyphs.Apple}", color, "Apple", index == 0 ? "+50 points, grows snake" : "same, colour is random")),
            Named($"{Glyphs.Heart}", theme.Heart, "Heart", "extra life bonus"),
            new(Frame(FrameStyle.Double), theme.Levels[0].Wall, "Wall", "level colour", "field border, deadly"),
            Named(Frame(FrameStyle.Single), theme.Hud.Frame, "HUD frame", "status panel border"),
            Named(Frame(FrameStyle.Double), theme.Menu.Frame, "Menu frame", "menus and dialogs"),
            Named($"{Glyphs.MenuMarker}", theme.Menu.Marker, "Menu marker", "selected menu item"),
            Named($"{Glyphs.InputCursor}", theme.Menu.InputCursor, "Input cursor", "name input, blinking"),
            Named($"{Glyphs.ArrowUp} {Glyphs.ArrowDown} {Glyphs.ArrowLeft} {Glyphs.ArrowRight}", theme.Hud.ControlKey, "Key names", "arrow keys in HUD"),
            new($"{Glyphs.FullBlock}{Glyphs.FullBlock}", theme.IntroLetters[^1], "Large letters", "rainbow", "intro, GET READY, YOU WIN!"),
            new(
                $"3 {Glyphs.MiddleDot} 2",
                theme.GetReady.CountdownOther,
                "Countdown",
                $"{theme.Describe(theme.GetReady.CountdownOther)}/{theme.Describe(theme.GetReady.CountdownCurrent)}",
                "under GET READY"),
            Named($"{Glyphs.RocketHead}", theme.Fireworks.Rocket, "Rocket head", "fireworks rocket"),
            Named($"{Glyphs.RocketTrail}", theme.Fireworks.Trail, "Rocket trail", "behind the rocket"),
            new($"{Glyphs.BurstCenter}", burst, "Burst centre", "burst colour", "fireworks explosion"),
            new($"{Glyphs.BurstInner} {Glyphs.BurstMiddle} {Glyphs.BurstOuter}", burst, "Particles", "fade to " + theme.Describe(theme.Fireworks.FadedParticle), "explosion rings"),
        ];
    }

    public string Title => "Glyphs and their purpose";

    public void Draw(FrameBuffer buffer)
    {
        Rgb label = _theme.Menu.Hint;
        buffer.Write(GlyphX, 2, "Glyph", label);
        buffer.Write(NameX, 2, "Object", label);
        buffer.Write(ColorX, 2, "Colour", label);
        buffer.Write(PurposeX, 2, "Purpose", label);

        for (var i = 0; i < _entries.Count; i++)
        {
            GlyphLegendEntry entry = _entries[i];
            int row = FirstRow + i;
            buffer.Write(GlyphX, row, entry.Glyphs, entry.GlyphColor);
            buffer.Write(NameX, row, entry.Name, _theme.Menu.Item);
            buffer.Write(ColorX, row, entry.ColorName, entry.GlyphColor);
            buffer.Write(PurposeX, row, entry.Purpose, label);
        }
    }

    private GlyphLegendEntry Named(string glyphs, Rgb color, string name, string purpose) =>
        new(glyphs, color, name, _theme.Describe(color), purpose);

    private static string Frame(FrameStyle style) =>
        $"{style.TopLeft}{style.Horizontal}{style.TopRight}{style.Vertical}{style.BottomLeft}{style.BottomRight}";
}
