using Zmeika.App.Storage.Dto;

namespace Zmeika.App.Storage;

internal static class TimingsFactory
{
    public static GameTimings Create(GameplayDto gameplay)
    {
        ArgumentNullException.ThrowIfNull(gameplay);

        return new GameTimings(
            Positive(gameplay.GetReadySeconds, "getReadySeconds"),
            Positive(gameplay.LevelFireworksSeconds, "levelFireworksSeconds"),
            Positive(gameplay.FinalFireworksSeconds, "finalFireworksSeconds"));
    }

    private static TimeSpan Positive(double seconds, string name) =>
        double.IsFinite(seconds) && seconds > 0
            ? TimeSpan.FromSeconds(seconds)
            : throw new ConfigException($"gameplay.{name} must be a positive number.");
}
