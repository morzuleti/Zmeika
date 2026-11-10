namespace Zmeika.App.Rendering;

internal readonly record struct Cell(char Glyph, Rgb? Foreground, Rgb? Background)
{
    public static Cell Empty { get; } = new(' ', null, null);
}
