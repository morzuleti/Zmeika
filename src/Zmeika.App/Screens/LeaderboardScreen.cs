using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;
using Zmeika.Core.Players;

namespace Zmeika.App.Screens;

internal sealed class LeaderboardScreen : IScreen
{
    private const string Retry = "Retry";
    private const string NewGame = "New game";
    private const string ChangePlayer = "Change player";
    private const string Exit = "Exit";
    private const string Back = "Back";
    private const int TableY = 2;
    private const int MenuY = 15;

    private readonly GameContext _context;
    private readonly GameSession _session;
    private readonly IScreen? _returnTo;
    private readonly IReadOnlyList<string> _items;
    private readonly MenuSelection _selection;
    private IReadOnlyList<LeaderboardRow> _rows = [];

    private LeaderboardScreen(GameContext context, GameSession session, IScreen? returnTo, IReadOnlyList<string> items)
    {
        _context = context;
        _session = session;
        _returnTo = returnTo;
        _items = items;
        _selection = new MenuSelection(items.Count);
    }

    public static LeaderboardScreen AfterGame(GameContext context, GameSession session, bool isWin)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(session);

        IReadOnlyList<string> items = isWin ? [NewGame, ChangePlayer, Exit] : [Retry, NewGame, ChangePlayer, Exit];
        return new LeaderboardScreen(context, session, returnTo: null, items);
    }

    public static LeaderboardScreen FromMenu(GameContext context, GameSession session, IScreen returnTo)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(returnTo);

        return new LeaderboardScreen(context, session, returnTo, [Back]);
    }

    public void OnEnter() => _rows = _context.Flow.Leaderboard(_context.Interface.LeaderboardSize);

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (_returnTo is not null && GameKeys.IsPause(key))
        {
            Activate(Back);
            return;
        }

        if (!_selection.TryMove(key) && GameKeys.IsConfirm(key))
        {
            Activate(_items[_selection.Index]);
        }
    }

    public void Update(TimeSpan elapsed)
    {
    }

    public void Paint(FrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        const string Title = "LEADERBOARD";
        Theme theme = _context.Theme;
        int tableX = ScreenLayout.CenterX(0, buffer.Width, LeaderboardPainter.Width);

        buffer.Write(ScreenLayout.CenterX(0, buffer.Width, Title.Length), 0, Title, theme.Leaderboard.Title);
        LeaderboardPainter.Paint(buffer, tableX, TableY, _rows, theme.Leaderboard);
        MenuPainter.Paint(buffer, tableX, MenuY, _items, _selection.Index, theme.Menu);
        buffer.Write(tableX + 2, buffer.Height - 1, $"{Glyphs.ArrowUp}{Glyphs.ArrowDown} choose  Enter ok", theme.Menu.Hint);
    }

    private void Activate(string item)
    {
        switch (item)
        {
            case Retry:
                _context.Flow.RetryAfterGameOver(_session);
                _context.Navigator.Show(new GetReadyScreen(_context, _session));
                break;
            case NewGame:
                _context.Navigator.Show(new GetReadyScreen(_context, _context.Flow.StartNewGameForCurrentPlayer()));
                break;
            case ChangePlayer:
                _context.Navigator.Show(new NameInputScreen(_context));
                break;
            case Exit:
                _context.Navigator.Exit();
                break;
            case Back:
                _context.Navigator.Show(_returnTo ?? throw new InvalidOperationException("There is no screen to return to."));
                break;
            default:
                throw new InvalidOperationException($"Unknown menu item {item}.");
        }
    }
}
