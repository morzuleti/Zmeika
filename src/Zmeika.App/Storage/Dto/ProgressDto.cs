namespace Zmeika.App.Storage.Dto;

internal sealed record ProgressDto(
    string Player,
    Guid GameId,
    int Level,
    int Lives,
    int Score);
