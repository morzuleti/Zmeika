namespace Zmeika.Core.Players;

public sealed record PlayerProgress(string Player, Guid GameId, int Level, int Lives, int Score);
