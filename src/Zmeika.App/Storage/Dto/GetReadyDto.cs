namespace Zmeika.App.Storage.Dto;

internal sealed record GetReadyDto(
    IReadOnlyList<string> LetterColors,
    string CountdownCurrent,
    string CountdownOther);
