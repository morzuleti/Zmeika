using System.Globalization;
using Zmeika.App.Audio;
using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;

namespace Zmeika.App.Screens;

internal sealed class OopsScreen(GameContext context, GameSession session) : IScreen
{
    private const int Height = 9;
    private static readonly string[] s_items = ["Retry"];

    public void OnEnter() => context.Audio.PlayJingle(SoundEffect.LifeLost);

    public void OnLeave() => context.Audio.StopMusic();

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (GameKeys.IsConfirm(key))
        {
            session.RestartLevel();
            context.Navigator.Show(new GetReadyScreen(context, session));
        }
    }

    public void Update(TimeSpan elapsed)
    {
    }

    public void Paint(FrameBuffer buffer)
    {
        LevelView.Paint(buffer, context, session);

        MenuTheme menu = context.Theme.Menu;
        int x = LevelView.DialogX;
        int y = LevelView.DialogY(Height);
        DialogPainter.Paint(buffer, x, y, LevelView.DialogWidth, Height, "OOPS!", menu);

        string livesText = string.Create(CultureInfo.InvariantCulture, $"{session.Lives} left");
        int livesX = ScreenLayout.CenterX(x, LevelView.DialogWidth, livesText.Length + 2);
        buffer.Put(livesX, y + 3, Glyphs.Heart, context.Theme.Heart);
        buffer.Write(livesX + 2, y + 3, livesText, menu.Item);

        MenuPainter.Paint(buffer, x, y + 5, s_items, 0, menu);
        buffer.Write(x + 2, y + 7, "Enter ok", menu.Hint);
    }
}
