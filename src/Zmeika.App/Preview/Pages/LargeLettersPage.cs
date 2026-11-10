using Zmeika.App.Rendering;

namespace Zmeika.App.Preview.Pages;

internal sealed class LargeLettersPage(Theme theme) : IPreviewPage
{
    private const string TitleText = "Zmeika";
    private const string GetReadyText = "GET READY";
    private const string YouWinText = "YOU WIN!";

    public string Title => "Large letters and countdown";

    public void Draw(FrameBuffer buffer)
    {
        Rgb label = theme.Menu.Hint;
        buffer.Write(2, 2, "Intro title, 7 rows", label);
        DrawCentered(buffer, LargeFonts.Title, 3, TitleText, theme.IntroLetters);

        buffer.Write(2, 12, "Get ready, 5 rows", label);
        DrawCentered(buffer, LargeFonts.Caption, 13, GetReadyText, theme.GetReady.LetterColors);
        DrawCountdown(buffer, 20);

        buffer.Write(2, 23, "Game complete, 5 rows", label);
        DrawCentered(buffer, LargeFonts.Caption, 24, YouWinText, theme.Fireworks.FinalCaptionColors);
    }

    private static void DrawCentered(FrameBuffer buffer, LargeFont font, int y, string text, IReadOnlyList<Rgb> colors)
    {
        int x = ScreenLayout.CenterX(0, buffer.Width, font.MeasureWidth(text));
        font.Draw(buffer, x, y, text, colors);
    }

    private void DrawCountdown(FrameBuffer buffer, int y)
    {
        string countdown = $"3 {Glyphs.MiddleDot} 2 {Glyphs.MiddleDot} 1";
        int x = ScreenLayout.CenterX(0, buffer.Width, countdown.Length);
        buffer.Write(x, y, countdown, theme.GetReady.CountdownOther);
        buffer.Put(x + 4, y, '2', theme.GetReady.CountdownCurrent);
        buffer.Write(2, y, "Countdown (2 is current)", theme.Menu.Hint);
    }
}
