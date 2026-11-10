using Zmeika.App.Input;
using Zmeika.App.Rendering;

namespace Zmeika.App.Storage;

internal sealed record PlayerSettings(string SnakeSkin, string SnakeColor, KeyBindings Keys)
{
    public static PlayerSettings Default(Theme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        return new PlayerSettings(theme.DefaultSkin.Name, theme.DefaultSnakeColor.Name, KeyBindings.Default);
    }

    public bool IsValidFor(Theme theme) =>
        FindSkin(theme) is not null && FindColor(theme) is not null && Keys.IsValid();

    public SnakeSkin ResolveSkin(Theme theme) => FindSkin(theme) ?? theme.DefaultSkin;

    public Rgb ResolveColor(Theme theme) => (FindColor(theme) ?? theme.DefaultSnakeColor).Color;

    private SnakeSkin? FindSkin(Theme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        return theme.Skins.FirstOrDefault(skin => string.Equals(skin.Name, SnakeSkin, StringComparison.OrdinalIgnoreCase));
    }

    private NamedColor? FindColor(Theme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        return theme.SnakeColors.FirstOrDefault(color => string.Equals(color.Name, SnakeColor, StringComparison.OrdinalIgnoreCase));
    }
}
