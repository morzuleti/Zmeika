namespace Zmeika.Core.Gameplay;

public sealed record LevelRules(int ApplesRequired, double CellsPerSecond, int? AppleRadius = null)
{
    public TimeSpan StepInterval => TimeSpan.FromSeconds(1 / CellsPerSecond);
}
