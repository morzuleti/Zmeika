namespace Zmeika.Core.Players;

public sealed record LeaderboardRow(int Place, string Player, int Score, bool IsCurrent);
