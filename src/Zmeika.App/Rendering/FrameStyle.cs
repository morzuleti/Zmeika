namespace Zmeika.App.Rendering;

internal sealed record FrameStyle(
    char TopLeft,
    char TopRight,
    char BottomLeft,
    char BottomRight,
    char Horizontal,
    char Vertical)
{
    public static FrameStyle Double { get; } = new(
        Glyphs.DoubleTopLeft,
        Glyphs.DoubleTopRight,
        Glyphs.DoubleBottomLeft,
        Glyphs.DoubleBottomRight,
        Glyphs.DoubleHorizontal,
        Glyphs.DoubleVertical);

    public static FrameStyle Single { get; } = new(
        Glyphs.SingleTopLeft,
        Glyphs.SingleTopRight,
        Glyphs.SingleBottomLeft,
        Glyphs.SingleBottomRight,
        Glyphs.SingleHorizontal,
        Glyphs.SingleVertical);
}
