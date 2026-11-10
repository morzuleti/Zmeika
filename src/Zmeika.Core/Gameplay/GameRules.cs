using System.Globalization;
using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

public sealed record GameRules(
    int InitialLives,
    int MaxLives,
    int InitialLength,
    int PointsPerApple,
    TimeSpan HeartSpawnMin,
    TimeSpan HeartSpawnMax,
    int AppleVariantCount,
    IReadOnlyList<LevelRules> Levels)
{
    public static FieldSize Field { get; } = new(30, 30);

    public int LevelCount => Levels.Count;

    public void Validate()
    {
        Require(InitialLives >= 1, "initialLives must be at least 1.");
        Require(MaxLives >= InitialLives, "maxLives must not be less than initialLives.");
        Require(InitialLength >= 1 && InitialLength <= Field.Rows, $"initialLength must be between 1 and {Field.Rows}.");
        Require(PointsPerApple >= 0, "pointsPerApple must not be negative.");
        Require(HeartSpawnMin > TimeSpan.Zero, "heartSpawnMinSeconds must be positive.");
        Require(HeartSpawnMax >= HeartSpawnMin, "heartSpawnMaxSeconds must not be less than heartSpawnMinSeconds.");
        Require(AppleVariantCount >= 1, "at least one apple colour is required.");
        Require(Levels.Count >= 1, "at least one level is required.");

        int maxApples = Field.CellCount - InitialLength - 2;
        for (var i = 0; i < Levels.Count; i++)
        {
            LevelRules level = Levels[i];
            string path = string.Create(CultureInfo.InvariantCulture, $"levels[{i}]");
            Require(
                level.ApplesRequired >= 1 && level.ApplesRequired <= maxApples,
                string.Create(CultureInfo.InvariantCulture, $"{path}.apples must be between 1 and {maxApples}."));
            Require(
                double.IsFinite(level.CellsPerSecond) && level.CellsPerSecond > 0,
                $"{path}.cellsPerSecond must be a positive number.");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new ArgumentException(message);
        }
    }
}
