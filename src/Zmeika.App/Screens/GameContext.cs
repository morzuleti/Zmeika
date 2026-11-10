using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.App.Storage;

namespace Zmeika.App.Screens;

internal sealed record GameContext(
    Theme Theme,
    GameTimings Timings,
    INavigator Navigator,
    InterfaceOptions Interface,
    SettingsStore SettingsStore,
    GameFlow Flow)
{
    public PlayerSettings Settings => SettingsStore.Current;

    public KeyBindings Keys => Settings.Keys;

    public SnakeSkin Skin => Settings.ResolveSkin(Theme);

    public Rgb SnakeColor => Settings.ResolveColor(Theme);
}
