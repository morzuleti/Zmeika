namespace Zmeika.App.Storage.Dto;

internal sealed record SnakeDto(
    IReadOnlyList<SkinDto> Skins,
    IReadOnlyList<string> Colors);
