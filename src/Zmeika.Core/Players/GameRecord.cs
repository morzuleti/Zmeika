namespace Zmeika.Core.Players;

public sealed record GameRecord(Guid Id, string Player, int Score, DateTimeOffset StartedAt, GameRecordStatus Status);
