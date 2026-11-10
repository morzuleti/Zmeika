namespace Zmeika.App.Storage.Dto;

internal sealed record GameConfigDto(
    IReadOnlyDictionary<string, string> Colors,
    GameplayDto Gameplay,
    IReadOnlyList<LevelDto> Levels,
    SnakeDto Snake,
    ObjectsDto Objects,
    HudDto Hud,
    MenuDto Menu,
    LeaderboardDto Leaderboard,
    IntroDto Intro,
    GetReadyDto GetReady,
    FireworksDto Fireworks,
    AudioDto Audio);
