namespace Zmeika.App.Rendering;

internal static class FireworksPainter
{
    private const int TrailLength = 3;

    private static readonly TimeSpan s_flashTime = TimeSpan.FromSeconds(0.1);

    public static void Paint(FrameBuffer buffer, FireworksSimulation simulation, FireworksTheme theme)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(simulation);
        ArgumentNullException.ThrowIfNull(theme);

        foreach (FireworksRocket rocket in simulation.Rockets)
        {
            int row = (int)Math.Round(rocket.Row);
            buffer.Put(rocket.Column, row, Glyphs.RocketHead, theme.Rocket);
            for (var i = 1; i <= TrailLength; i++)
            {
                buffer.Put(rocket.Column, row + i, Glyphs.RocketTrail, theme.Trail);
            }
        }

        foreach (FireworksParticle particle in simulation.Particles)
        {
            (char glyph, Rgb color) = Look(particle, theme);
            buffer.Put((int)Math.Round(particle.Column), (int)Math.Round(particle.Row), glyph, color);
        }
    }

    private static (char Glyph, Rgb Color) Look(FireworksParticle particle, FireworksTheme theme)
    {
        double life = particle.Age / FireworksSimulation.ParticleLifetime;
        return particle.Age < s_flashTime
            ? (Glyphs.BurstCenter, particle.Color)
            : life switch
            {
                < 0.4 => (Glyphs.BurstInner, particle.Color),
                < 2.0 / 3 => (Glyphs.BurstMiddle, particle.Color),
                _ => (Glyphs.BurstOuter, theme.FadedParticle),
            };
    }
}
