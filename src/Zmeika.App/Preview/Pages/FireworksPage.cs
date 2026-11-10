using Zmeika.App.Rendering;

namespace Zmeika.App.Preview.Pages;

internal sealed class FireworksPage(Theme theme) : IPreviewPage
{
    private const int HalfWidth = 44;
    private const int BurstRadius = 3;
    private const int ParticlesPerRing = 8;
    private const int RandomSeed = 7;

    private FireworksTheme Fireworks => theme.Fireworks;

    public string Title => "Fireworks";

    public void Draw(FrameBuffer buffer)
    {
        DrawLevelComplete(buffer);
        DrawGameComplete(buffer);
    }

    private void DrawLevelComplete(FrameBuffer buffer)
    {
        const string Caption = "LEVEL 1 COMPLETE!";
        buffer.Write(ScreenLayout.CenterX(0, HalfWidth, Caption.Length), 3, Caption, Fireworks.LevelCaption);

        IReadOnlyList<Rgb> colors = Fireworks.LevelBurstColors;
        DrawBurst(buffer, 11, 10, _ => colors[0]);
        DrawBurst(buffer, 31, 9, _ => colors[3 % colors.Count]);
        DrawBurst(buffer, 21, 18, _ => colors[4 % colors.Count]);
        DrawRocket(buffer, 37, 22);
        buffer.Write(2, 29, "Level complete, 5 s, one colour per burst", theme.Menu.Hint);
    }

    private void DrawGameComplete(FrameBuffer buffer)
    {
        const string Caption = "YOU WIN!";
        int width = LargeFonts.Caption.MeasureWidth(Caption);
        LargeFonts.Caption.Draw(buffer, ScreenLayout.CenterX(HalfWidth, HalfWidth, width), 2, Caption, Fireworks.FinalCaptionColors);

        Random random = new(RandomSeed);
        Rgb RandomColor(int ring) => Fireworks.FinalBurstColors[random.Next(Fireworks.FinalBurstColors.Count)];
        DrawBurst(buffer, 54, 13, RandomColor);
        DrawBurst(buffer, 76, 12, RandomColor);
        DrawBurst(buffer, 65, 21, RandomColor);
        DrawRocket(buffer, 49, 24);
        DrawRocket(buffer, 82, 23);
        buffer.Write(HalfWidth + 2, 29, "Game complete, 15 s, mixed colours", theme.Menu.Hint);
    }

    private void DrawBurst(FrameBuffer buffer, int centerX, int centerY, Func<int, Rgb> colorForRing)
    {
        buffer.Put(centerX, centerY, Glyphs.BurstCenter, colorForRing(0));
        for (var ring = 1; ring <= BurstRadius; ring++)
        {
            char glyph = ring switch
            {
                1 => Glyphs.BurstInner,
                2 => Glyphs.BurstMiddle,
                _ => Glyphs.BurstOuter,
            };

            for (var particle = 0; particle < ParticlesPerRing; particle++)
            {
                double angle = 2 * Math.PI * particle / ParticlesPerRing;
                int x = centerX + (int)Math.Round(ScreenLayout.CellWidth * ring * Math.Cos(angle));
                int y = centerY + (int)Math.Round(ring * Math.Sin(angle));
                Rgb color = ring == BurstRadius ? Fireworks.FadedParticle : colorForRing(ring);
                buffer.Put(x, y, glyph, color);
            }
        }
    }

    private void DrawRocket(FrameBuffer buffer, int x, int y)
    {
        const int TrailLength = 3;
        buffer.Put(x, y, Glyphs.RocketHead, Fireworks.Rocket);
        for (var i = 1; i <= TrailLength; i++)
        {
            buffer.Put(x, y + i, Glyphs.RocketTrail, Fireworks.Trail);
        }
    }
}
