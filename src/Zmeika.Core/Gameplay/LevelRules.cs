namespace Zmeika.Core.Gameplay;

public sealed record LevelRules(int ApplesRequired, double CellsPerSecond)
{
    public TimeSpan StepInterval => TimeSpan.FromSeconds(1 / CellsPerSecond);
}
