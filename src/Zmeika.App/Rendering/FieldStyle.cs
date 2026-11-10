namespace Zmeika.App.Rendering;

internal sealed record FieldStyle(
    LevelColorScheme Scheme,
    SnakeSkin Skin,
    Rgb SnakeColor,
    IReadOnlyList<Rgb> AppleColors,
    Rgb Heart);
