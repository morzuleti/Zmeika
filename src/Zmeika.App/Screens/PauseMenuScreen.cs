using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;

namespace Zmeika.App.Screens;

internal sealed class PauseMenuScreen(GameContext context, GameSession session, IScreen resume) : IScreen
{
    private const int Height = 10;
    private const int ContinueItem = 0;
    private const int NewGameItem = 1;
    private const int SettingsItem = 2;
    private const int ExitItem = 3;

    private static readonly string[] s_items = ["Continue", "New game", "Settings", "Exit"];
    private static readonly TimeSpan s_hintDuration = TimeSpan.FromSeconds(2);

    private readonly MenuSelection _selection = new(s_items.Length);
    private TimeSpan _settingsHintLeft;

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (GameKeys.IsPause(key) || GameKeys.IsBack(key))
        {
            context.Navigator.Show(resume);
            return;
        }

        if (!_selection.TryMove(key) && GameKeys.IsConfirm(key))
        {
            Activate(_selection.Index);
        }
    }

    public void Update(TimeSpan elapsed)
    {
        if (_settingsHintLeft > TimeSpan.Zero)
        {
            _settingsHintLeft -= elapsed;
        }
    }

    public void Paint(FrameBuffer buffer)
    {
        LevelView.Paint(buffer, context, session);

        MenuTheme menu = context.Theme.Menu;
        int x = LevelView.DialogX;
        int y = LevelView.DialogY(Height);
        DialogPainter.Paint(buffer, x, y, LevelView.DialogWidth, Height, "MENU", menu);
        MenuPainter.Paint(buffer, x, y + 3, s_items, _selection.Index, menu);

        string hint = _settingsHintLeft > TimeSpan.Zero
            ? "Settings: coming soon"
            : $"{Glyphs.ArrowUp}{Glyphs.ArrowDown} choose  Enter ok";
        buffer.Write(x + 2, y + 8, hint, menu.Hint);
    }

    private void Activate(int item)
    {
        switch (item)
        {
            case ContinueItem:
                context.Navigator.Show(resume);
                break;
            case NewGameItem:
                context.Navigator.Show(new GetReadyScreen(context, context.Flow.StartNewGameForCurrentPlayer()));
                break;
            case SettingsItem:
                _settingsHintLeft = s_hintDuration;
                break;
            case ExitItem:
                context.Navigator.Exit();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(item), item, "Unknown menu item.");
        }
    }
}
