using System.Globalization;
using Zmeika.App.Rendering;
using Zmeika.App.Storage.Dto;

namespace Zmeika.App.Storage;

internal static class ThemeFactory
{
    public static Theme Create(GameConfigDto document)
    {
        ArgumentNullException.ThrowIfNull(document);

        IReadOnlyList<NamedColor> palette = CreatePalette(document.Colors);
        ColorResolver colors = new(palette);

        return new Theme(
            palette,
            CreateLevels(document.Levels, colors),
            CreateSkins(document.Snake.Skins),
            CreateSnakeColors(document.Snake.Colors, colors),
            colors.ResolveAll(document.Objects.AppleColors, "objects.appleColors"),
            colors.Resolve(document.Objects.Heart, "objects.heart"),
            CreateHud(document.Hud, colors),
            CreateMenu(document.Menu, colors),
            CreateLeaderboard(document.Leaderboard, colors),
            colors.ResolveAll(document.Intro.LetterColors, "intro.letterColors"),
            CreateGetReady(document.GetReady, colors),
            CreateFireworks(document.Fireworks, colors));
    }

    private static List<NamedColor> CreatePalette(IReadOnlyDictionary<string, string> colors)
    {
        if (colors.Count == 0)
        {
            throw new ConfigException("colors: the palette must contain at least one colour.");
        }

        HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);
        List<NamedColor> palette = new(colors.Count);
        foreach (KeyValuePair<string, string> entry in colors)
        {
            string path = $"colors.{entry.Key}";
            if (!names.Add(entry.Key))
            {
                throw new ConfigException($"{path}: the colour name is used twice.");
            }

            if (!Rgb.TryFromHex(entry.Value, out Rgb color))
            {
                throw new ConfigException($"{path}: '{entry.Value}' is not a #RRGGBB colour.");
            }

            palette.Add(new NamedColor(entry.Key, color));
        }

        return palette;
    }

    private static LevelColorScheme[] CreateLevels(IReadOnlyList<LevelDto> levels, ColorResolver colors)
    {
        if (levels.Count == 0)
        {
            throw new ConfigException("levels: at least one level is required.");
        }

        LevelColorScheme[] schemes = new LevelColorScheme[levels.Count];
        for (var i = 0; i < levels.Count; i++)
        {
            LevelDto level = levels[i];
            string path = string.Create(CultureInfo.InvariantCulture, $"levels[{i}]");
            schemes[i] = new LevelColorScheme(
                i + 1,
                level.Wall,
                colors.Resolve(level.Wall, $"{path}.wall"),
                colors.Resolve(level.ShadeA, $"{path}.shadeA"),
                colors.Resolve(level.ShadeB, $"{path}.shadeB"));
        }

        return schemes;
    }

    private static SnakeSkin[] CreateSkins(IReadOnlyList<SkinDto> skins)
    {
        if (skins.Count == 0)
        {
            throw new ConfigException("snake.skins: at least one skin is required.");
        }

        SnakeSkin[] result = new SnakeSkin[skins.Count];
        for (var i = 0; i < skins.Count; i++)
        {
            SkinDto skin = skins[i];
            string path = string.Create(CultureInfo.InvariantCulture, $"snake.skins[{i}]");
            result[i] = new SnakeSkin(skin.Name, SingleGlyph(skin.Head, $"{path}.head"), SingleGlyph(skin.Body, $"{path}.body"));
        }

        return result;
    }

    private static char SingleGlyph(string value, string path) =>
        value.Length == 1
            ? value[0]
            : throw new ConfigException($"{path}: '{value}' must be exactly one character.");

    private static NamedColor[] CreateSnakeColors(IReadOnlyList<string> names, ColorResolver colors)
    {
        IReadOnlyList<Rgb> resolved = colors.ResolveAll(names, "snake.colors");
        NamedColor[] result = new NamedColor[names.Count];
        for (var i = 0; i < names.Count; i++)
        {
            result[i] = new NamedColor(names[i], resolved[i]);
        }

        return result;
    }

    private static HudTheme CreateHud(HudDto hud, ColorResolver colors) => new(
        colors.Resolve(hud.Frame, "hud.frame"),
        colors.Resolve(hud.Level, "hud.level"),
        colors.Resolve(hud.Label, "hud.label"),
        colors.Resolve(hud.PlayerName, "hud.playerName"),
        colors.Resolve(hud.Heart, "hud.heart"),
        colors.Resolve(hud.Apple, "hud.apple"),
        colors.Resolve(hud.Value, "hud.value"),
        colors.Resolve(hud.Score, "hud.score"),
        colors.Resolve(hud.ControlsCaption, "hud.controlsCaption"),
        colors.Resolve(hud.ControlDirection, "hud.controlDirection"),
        colors.Resolve(hud.ControlKey, "hud.controlKey"));

    private static MenuTheme CreateMenu(MenuDto menu, ColorResolver colors) => new(
        colors.Resolve(menu.Frame, "menu.frame"),
        colors.Resolve(menu.Title, "menu.title"),
        colors.Resolve(menu.Item, "menu.item"),
        colors.Resolve(menu.SelectedItem, "menu.selectedItem"),
        colors.Resolve(menu.Marker, "menu.marker"),
        colors.Resolve(menu.Hint, "menu.hint"),
        colors.Resolve(menu.PlayerName, "menu.playerName"),
        colors.Resolve(menu.InputText, "menu.inputText"),
        colors.Resolve(menu.InputCursor, "menu.inputCursor"));

    private static LeaderboardTheme CreateLeaderboard(LeaderboardDto leaderboard, ColorResolver colors) => new(
        colors.Resolve(leaderboard.Title, "leaderboard.title"),
        colors.Resolve(leaderboard.Header, "leaderboard.header"),
        colors.Resolve(leaderboard.FirstPlace, "leaderboard.firstPlace"),
        colors.Resolve(leaderboard.CurrentGame, "leaderboard.currentGame"),
        colors.Resolve(leaderboard.OtherGames, "leaderboard.otherGames"));

    private static GetReadyTheme CreateGetReady(GetReadyDto getReady, ColorResolver colors) => new(
        colors.ResolveAll(getReady.LetterColors, "getReady.letterColors"),
        colors.Resolve(getReady.CountdownCurrent, "getReady.countdownCurrent"),
        colors.Resolve(getReady.CountdownOther, "getReady.countdownOther"));

    private static FireworksTheme CreateFireworks(FireworksDto fireworks, ColorResolver colors) => new(
        colors.Resolve(fireworks.LevelCaption, "fireworks.levelCaption"),
        colors.ResolveAll(fireworks.LevelBurstColors, "fireworks.levelBurstColors"),
        colors.ResolveAll(fireworks.FinalCaptionColors, "fireworks.finalCaptionColors"),
        colors.ResolveAll(fireworks.FinalBurstColors, "fireworks.finalBurstColors"),
        colors.Resolve(fireworks.Rocket, "fireworks.rocket"),
        colors.Resolve(fireworks.Trail, "fireworks.trail"),
        colors.Resolve(fireworks.FadedParticle, "fireworks.fadedParticle"));
}
