namespace Zmeika.App.Storage.Dto;

internal sealed record PlayersDto(
    IReadOnlyList<GameRecordDto> Games,
    IReadOnlyList<ProgressDto> Progress);
