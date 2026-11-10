using Zmeika.App.Rendering;
using Zmeika.App.Storage.Dto;

namespace Zmeika.App.Storage;

internal sealed record GameConfig(GameConfigDto Document, Theme Theme);
