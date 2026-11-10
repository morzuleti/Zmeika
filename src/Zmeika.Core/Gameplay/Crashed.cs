namespace Zmeika.Core.Gameplay;

public sealed record Crashed(DeathCause Cause) : LevelEvent;
