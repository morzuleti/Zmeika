using Zmeika.App.Rendering;
using Zmeika.App.Storage.Dto;
using Zmeika.Core.Gameplay;

namespace Zmeika.App.Storage;

internal sealed record GameConfig(
    GameConfigDto Document,
    Theme Theme,
    GameRules Rules,
    GameTimings Timings,
    InterfaceOptions Interface);
