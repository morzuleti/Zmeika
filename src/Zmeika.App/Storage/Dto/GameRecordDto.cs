using Zmeika.Core.Players;

namespace Zmeika.App.Storage.Dto;

internal sealed record GameRecordDto(
    Guid Id,
    string Player,
    int Score,
    DateTimeOffset StartedAt,
    GameRecordStatus Status);
