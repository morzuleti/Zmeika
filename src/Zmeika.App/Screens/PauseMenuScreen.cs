using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;

namespace Zmeika.App.Screens;

internal sealed class PauseMenuScreen(GameContext context, GameSession session, IScreen resume) : IScreen
{
    private const int Height = 11;
    private const int ContinueItem = 0;
    private const int NewGameItem = 1;
    private const int LeaderboardItem = 2;
    private const int SettingsItem = 3;
    private const int ExitItem = 4;

    private static readonly string[] s_items = ["Continue", "New game", "Leaderboard", "Settings", "Exit"];

    private readonly MenuSelection _selection = new(s_items.Length);

    public void OnEnter() => context.Audio.PauseMusic();

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (GameKeys.IsPause(key) || GameKeys.IsBack(key))
        {
            ResumeGame();
            return;
        }

        if (!_selection.TryMove(key) && GameKeys.IsConfirm(key))
        {
            Activate(_selection.Index);
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
        DialogPainter.Paint(buffer, x, y, LevelView.DialogWidth, Height, "MENU", menu);
        MenuPainter.Paint(buffer, x, y + 3, s_items, _selection.Index, menu);

        buffer.Write(x + 2, y + Height - 2, $"{Glyphs.ArrowUp}{Glyphs.ArrowDown} choose  Enter ok", menu.Hint);
    }

    private void Activate(int item)
    {
        switch (item)
        {
            case ContinueItem:
                ResumeGame();
                break;
            case NewGameItem:
                context.Navigator.Show(new GetReadyScreen(context, context.Flow.StartNewGameForCurrentPlayer()));
                break;
            case LeaderboardItem:
                context.Navigator.Show(LeaderboardScreen.FromMenu(context, session, this));
                break;
            case SettingsItem:
                context.Navigator.Show(new SettingsScreen(context, session, this));
                break;
            case ExitItem:
                context.Navigator.Exit();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(item), item, "Unknown menu item.");
        }
    }

    private void ResumeGame()
    {
        context.Audio.ResumeMusic();
        context.Navigator.Show(resume);
    }
}
