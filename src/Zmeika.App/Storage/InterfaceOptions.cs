using Zmeika.App.Storage.Dto;

namespace Zmeika.App.Storage;

internal sealed record InterfaceOptions(int MaxNameLength, int LeaderboardSize)
{
    public const int LongestFittingName = 18;

    public static InterfaceOptions Create(GameplayDto gameplay)
    {
        ArgumentNullException.ThrowIfNull(gameplay);

        if (gameplay.MaxNameLength is < 1 or > LongestFittingName)
        {
            throw new ConfigException($"gameplay.maxNameLength must be between 1 and {LongestFittingName}.");
        }

        if (gameplay.LeaderboardSize < 1)
        {
            throw new ConfigException("gameplay.leaderboardSize must be at least 1.");
        }

        return new InterfaceOptions(gameplay.MaxNameLength, gameplay.LeaderboardSize);
    }
}
