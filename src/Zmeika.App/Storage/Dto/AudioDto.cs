namespace Zmeika.App.Storage.Dto;

internal sealed record AudioDto(
    double MusicVolume,
    double EffectsVolume,
    SoundsDto Sounds);
