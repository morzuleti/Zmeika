namespace Zmeika.Core.Gameplay;

public sealed record SnakeDied(DeathCause Cause, int LivesLeft) : GameEvent;
