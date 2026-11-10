namespace Zmeika.App.Storage.Dto;

internal sealed record LevelDto(
    int Apples,
    double CellsPerSecond,
    string Music,
    string Wall,
    string ShadeA,
    string ShadeB,
    int? AppleRadius = null);
