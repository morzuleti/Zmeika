using Zmeika.App.Storage.Dto;
using Zmeika.Core.Gameplay;

namespace Zmeika.App.Storage;

internal static class RulesFactory
{
    public static GameRules Create(GameConfigDto document)
    {
        ArgumentNullException.ThrowIfNull(document);

        GameplayDto gameplay = document.Gameplay;
        GameRules rules = new(
            gameplay.InitialLives,
            gameplay.MaxLives,
            gameplay.InitialLength,
            gameplay.PointsPerApple,
            TimeSpan.FromSeconds(gameplay.HeartSpawnMinSeconds),
            TimeSpan.FromSeconds(gameplay.HeartSpawnMaxSeconds),
            TimeSpan.FromSeconds(gameplay.HeartLifetimeSeconds),
            TimeSpan.FromSeconds(gameplay.HeartBlinkSeconds),
            document.Objects.AppleColors.Count,
            [.. document.Levels.Select(level => new LevelRules(level.Apples, level.CellsPerSecond, level.AppleRadius))]);

        try
        {
            rules.Validate();
        }
        catch (ArgumentException exception)
        {
            throw new ConfigException(exception.Message, exception);
        }

        return rules;
    }
}
