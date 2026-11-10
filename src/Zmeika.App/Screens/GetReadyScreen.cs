using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;

namespace Zmeika.App.Screens;

internal sealed class GetReadyScreen(GameContext context, GameSession session) : IScreen
{
    private const string Caption = "GET READY";
    private const int CountdownGap = 2;

    private TimeSpan _elapsed;

    public void OnEnter()
    {
        context.Flow.OnLevelStarted(session);
        context.Audio.PlayLevelMusic(session.Level);
    }

    public void HandleKey(ConsoleKeyInfo key)
    {
    }

    public void Update(TimeSpan elapsed)
    {
        _elapsed += elapsed;
        if (_elapsed >= context.Timings.GetReady)
        {
            context.Navigator.Show(new LevelScreen(context, session));
        }
    }

    public void Paint(FrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        LargeFont font = LargeFonts.Caption;
        int captionY = (buffer.Height - font.Height - CountdownGap - 1) / 2;
        int captionX = ScreenLayout.CenterX(0, buffer.Width, font.MeasureWidth(Caption));
        font.Draw(buffer, captionX, captionY, Caption, context.Theme.GetReady.LetterColors);

        int total = (int)Math.Ceiling(context.Timings.GetReady.TotalSeconds);
        int current = Math.Max(1, total - (int)_elapsed.TotalSeconds);
        CountdownPainter.Paint(buffer, 0, buffer.Width, captionY + font.Height + CountdownGap, total, current, context.Theme.GetReady);
    }
}
