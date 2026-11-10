using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;
using Zmeika.Core.Players;

namespace Zmeika.App.Screens;

internal sealed class WelcomeBackScreen(GameContext context, PlayerName player, PlayerProgress progress) : IScreen
{
    private const int Height = 10;
    private const int ContinueItem = 0;

    private static readonly string[] s_items = ["Continue", "New game"];

    private readonly MenuSelection _selection = new(s_items.Length);

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (_selection.TryMove(key) || !GameKeys.IsConfirm(key))
        {
            return;
        }

        GameFlow flow = context.Flow;
        GameSession session = _selection.Index == ContinueItem
            ? flow.ContinueGame(player, progress)
            : flow.StartNewGame(player);
        context.Navigator.Show(new GetReadyScreen(context, session));
    }

    public void Update(TimeSpan elapsed)
    {
    }

    public void Paint(FrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        MenuTheme menu = context.Theme.Menu;
        int x = ScreenLayout.CenterX(0, buffer.Width, LevelView.DialogWidth);
        int y = (buffer.Height - Height) / 2;
        DialogPainter.Paint(buffer, x, y, LevelView.DialogWidth, Height, "WELCOME BACK", menu);
        buffer.Write(ScreenLayout.CenterX(x, LevelView.DialogWidth, player.Value.Length), y + 2, player.Value, menu.PlayerName);
        MenuPainter.Paint(buffer, x, y + 4, s_items, _selection.Index, menu);
        buffer.Write(x + 2, y + 8, $"{Glyphs.ArrowUp}{Glyphs.ArrowDown} choose  Enter ok", menu.Hint);
    }
}
