namespace Zmeika.App.Rendering;

internal sealed record FireworksTheme(
    Rgb LevelCaption,
    IReadOnlyList<Rgb> LevelBurstColors,
    IReadOnlyList<Rgb> FinalCaptionColors,
    IReadOnlyList<Rgb> FinalBurstColors,
    Rgb Rocket,
    Rgb Trail,
    Rgb FadedParticle);
