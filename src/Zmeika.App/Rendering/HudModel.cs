namespace Zmeika.App.Rendering;

internal sealed record HudModel(
    int Level,
    int LevelCount,
    string Player,
    int Lives,
    int ApplesEaten,
    int ApplesRequired,
    int Score,
    HudControls Controls,
    string? Warning = null);
