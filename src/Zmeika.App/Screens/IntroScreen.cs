using Zmeika.App.Rendering;

namespace Zmeika.App.Screens;

internal sealed class IntroScreen(GameContext context) : IScreen
{
    private const string Title = "Zmeika";
    private const double FallRowsPerSecond = 20;

    private static readonly TimeSpan s_holdAfterLanding = TimeSpan.FromSeconds(0.7);

    private static readonly LetterBounce[] s_bounces =
    [
        new(Height: 3, Duration: TimeSpan.FromSeconds(0.4)),
        new(Height: 1, Duration: TimeSpan.FromSeconds(0.22)),
    ];

    private readonly LargeFont _font = LargeFonts.Title;
    private TimeSpan _elapsed;

    private int StartY => -_font.Height;

    private int TargetY => (ScreenLayout.Height - _font.Height) / 2;

    private TimeSpan FallTime => TimeSpan.FromSeconds((TargetY - StartY) / FallRowsPerSecond);

    private TimeSpan LetterAnimationTime => FallTime + s_bounces.Aggregate(TimeSpan.Zero, (total, bounce) => total + bounce.Duration);

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Spacebar)
        {
            Finish();
        }
    }

    public void Update(TimeSpan elapsed)
    {
        _elapsed += elapsed;
        int lastLetter = _font.Layout(Title).Count - 1;
        if (_elapsed >= (FallTime * lastLetter) + LetterAnimationTime + s_holdAfterLanding)
        {
            Finish();
        }
    }

    public void Paint(FrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        IReadOnlyList<LetterPlacement> letters = _font.Layout(Title);
        IReadOnlyList<Rgb> colors = context.Theme.IntroLetters;
        int x = ScreenLayout.CenterX(0, buffer.Width, _font.MeasureWidth(Title));

        for (var i = 0; i < letters.Count; i++)
        {
            TimeSpan sinceStart = _elapsed - (FallTime * i);
            if (sinceStart < TimeSpan.Zero)
            {
                break;
            }

            _font.DrawLetter(buffer, x + letters[i].OffsetX, LetterY(sinceStart), letters[i].Letter, colors[i % colors.Count]);
        }
    }

    private int LetterY(TimeSpan sinceStart)
    {
        if (sinceStart < FallTime)
        {
            return StartY + (int)(sinceStart.TotalSeconds * FallRowsPerSecond);
        }

        TimeSpan sinceLanding = sinceStart - FallTime;
        foreach (LetterBounce bounce in s_bounces)
        {
            if (sinceLanding < bounce.Duration)
            {
                double progress = sinceLanding / bounce.Duration;
                double lift = bounce.Height * 4 * progress * (1 - progress);
                return TargetY - (int)Math.Round(lift);
            }

            sinceLanding -= bounce.Duration;
        }

        return TargetY;
    }

    private void Finish() => context.Navigator.Show(new NameInputScreen(context));
}
