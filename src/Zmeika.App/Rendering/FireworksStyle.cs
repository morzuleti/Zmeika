namespace Zmeika.App.Rendering;

internal sealed record FireworksStyle(TimeSpan LaunchInterval, int RocketsPerLaunch, bool MixedParticleColors)
{
    public static FireworksStyle LevelComplete { get; } = new(TimeSpan.FromSeconds(0.6), RocketsPerLaunch: 1, MixedParticleColors: false);

    public static FireworksStyle GameComplete { get; } = new(TimeSpan.FromSeconds(0.3), RocketsPerLaunch: 2, MixedParticleColors: true);
}
