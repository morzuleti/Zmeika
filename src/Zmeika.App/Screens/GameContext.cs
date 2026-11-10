using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.App.Storage;

namespace Zmeika.App.Screens;

internal sealed record GameContext(
    Theme Theme,
    GameTimings Timings,
    INavigator Navigator,
    InterfaceOptions Interface,
    PlayerSettings Settings,
    GameFlow Flow)
{
    public KeyBindings Keys => Settings.Keys;

    public SnakeSkin Skin => Settings.ResolveSkin(Theme);

    public Rgb SnakeColor => Settings.ResolveColor(Theme);
}
