namespace Zmeika.App.Storage.Dto;

internal sealed record FireworksDto(
    string LevelCaption,
    IReadOnlyList<string> LevelBurstColors,
    IReadOnlyList<string> FinalCaptionColors,
    IReadOnlyList<string> FinalBurstColors,
    string Rocket,
    string Trail,
    string FadedParticle);
