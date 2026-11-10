namespace Zmeika.Core.Gameplay;

public sealed record LevelCompleted(int Level, bool IsLastLevel) : GameEvent;
