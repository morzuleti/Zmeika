using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.App.Storage;
using Zmeika.Core.Gameplay;

namespace Zmeika.App.Screens;

internal sealed record GameContext(
    GameRules Rules,
    Theme Theme,
    GameTimings Timings,
    IRandomSource Random,
    INavigator Navigator,
    KeyBindings Keys,
    string PlayerName);
