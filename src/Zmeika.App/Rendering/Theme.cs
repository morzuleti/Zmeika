namespace Zmeika.App.Rendering;

internal sealed record Theme(
    IReadOnlyList<NamedColor> Palette,
    IReadOnlyList<LevelColorScheme> Levels,
    IReadOnlyList<SnakeSkin> Skins,
    IReadOnlyList<NamedColor> SnakeColors,
    IReadOnlyList<Rgb> AppleColors,
    Rgb Heart,
    HudTheme Hud,
    MenuTheme Menu,
    LeaderboardTheme Leaderboard,
    IReadOnlyList<Rgb> IntroLetters,
    GetReadyTheme GetReady,
    FireworksTheme Fireworks)
{
    public SnakeSkin DefaultSkin => Skins[0];

    public NamedColor DefaultSnakeColor => SnakeColors[0];

    public string Describe(Rgb color)
    {
        foreach (NamedColor named in Palette)
        {
            if (named.Color == color)
            {
                return named.Name;
            }
        }

        return color.ToHex();
    }
}
