namespace Zmeika.App.Storage.Dto;

internal sealed record GameplayDto(
    int InitialLives,
    int MaxLives,
    int InitialBodyLength,
    int PointsPerApple,
    double HeartSpawnMinSeconds,
    double HeartSpawnMaxSeconds,
    double GetReadySeconds,
    double LevelFireworksSeconds,
    double FinalFireworksSeconds,
    int LeaderboardSize,
    int MaxNameLength);
