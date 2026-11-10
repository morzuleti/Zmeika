using System.Globalization;
using Zmeika.App.Audio;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;

namespace Zmeika.App.Screens;

internal static class FireworksScreens
{
    private const string WinCaption = "YOU WIN!";
    private const int CaptionRow = 1;

    public static IScreen LevelComplete(GameContext context, GameSession session)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(session);

        FireworksTheme theme = context.Theme.Fireworks;
        string caption = string.Create(CultureInfo.InvariantCulture, $"LEVEL {session.Level} COMPLETE!");
        return new FireworksScreen(
            Simulation(FireworksStyle.LevelComplete, theme.LevelBurstColors),
            theme,
            context.Timings.LevelFireworks,
            context.Audio,
            SoundEffect.LevelComplete,
            buffer => buffer.Write(ScreenLayout.CenterX(0, buffer.Width, caption.Length), CaptionRow, caption, theme.LevelCaption),
            () =>
            {
                session.AdvanceToNextLevel();
                context.Navigator.Show(new GetReadyScreen(context, session));
            });
    }

    public static IScreen GameComplete(GameContext context, GameSession session)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(session);

        FireworksTheme theme = context.Theme.Fireworks;
        LargeFont font = LargeFonts.Caption;
        return new FireworksScreen(
            Simulation(FireworksStyle.GameComplete, theme.FinalBurstColors),
            theme,
            context.Timings.FinalFireworks,
            context.Audio,
            SoundEffect.Victory,
            buffer => font.Draw(buffer, ScreenLayout.CenterX(0, buffer.Width, font.MeasureWidth(WinCaption)), CaptionRow, WinCaption, theme.FinalCaptionColors),
            () => context.Navigator.Show(LeaderboardScreen.AfterGame(context, session, isWin: true)));
    }

    private static FireworksSimulation Simulation(FireworksStyle style, IReadOnlyList<Rgb> colors) =>
        new(style, colors, new Random(), ScreenLayout.Width, ScreenLayout.Height);
}
