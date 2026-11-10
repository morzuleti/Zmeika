using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;
using Zmeika.Core.Model;

namespace Zmeika.App.Screens;

internal sealed class LevelScreen : IScreen
{
    private const string PlaceholderPlayer = "Player";

    private readonly Theme _theme;
    private readonly INavigator _navigator;
    private readonly KeyBindings _keys = KeyBindings.Default;
    private readonly GameSession _session;

    public LevelScreen(GameRules rules, Theme theme, IRandomSource random, INavigator navigator)
    {
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentNullException.ThrowIfNull(navigator);

        _theme = theme;
        _navigator = navigator;
        _session = GameSession.NewGame(rules, random);
    }

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (IsExitKey(key))
        {
            _navigator.Exit();
            return;
        }

        if (_keys.TryGetDirection(key.Key, out Direction direction))
        {
            _session.QueueTurn(direction);
        }
    }

    public void Update(TimeSpan elapsed)
    {
        _session.Update(elapsed);

        switch (_session.Status)
        {
            case GameStatus.Died:
                _session.RestartLevel();
                break;
            case GameStatus.LevelCompleted:
                _session.AdvanceToNextLevel();
                break;
            case GameStatus.Playing:
            case GameStatus.GameOver:
            case GameStatus.Won:
                break;
            default:
                throw new InvalidOperationException($"Unknown game status {_session.Status}.");
        }
    }

    public void Paint(FrameBuffer buffer)
    {
        LevelSession level = _session.Current;
        FieldStyle style = new(
            _theme.Levels[level.Level - 1],
            _theme.DefaultSkin,
            _theme.DefaultSnakeColor.Color,
            _theme.AppleColors,
            _theme.Heart);
        FieldPainter.Paint(buffer, level, style);

        HudModel hud = new(
            level.Level,
            _session.Rules.LevelCount,
            PlaceholderPlayer,
            _session.Lives,
            level.ApplesEaten,
            level.ApplesRequired,
            _session.Score,
            KeyNames.ForHud(_keys));
        HudPainter.Paint(buffer, hud, _theme.Hud);

        if (_session.Status is GameStatus.GameOver or GameStatus.Won)
        {
            PaintFinalMessage(buffer, _session.Status == GameStatus.Won ? "YOU WIN!" : "GAME OVER");
        }
    }

    private static bool IsExitKey(ConsoleKeyInfo key) =>
        key.Key == ConsoleKey.Escape
        || (key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control));

    private void PaintFinalMessage(FrameBuffer buffer, string title)
    {
        const string Hint = "Esc exit";
        const int FieldWidth = (ScreenLayout.FieldColumns * ScreenLayout.CellWidth) + 2;
        int centerRow = ScreenLayout.FieldY + (ScreenLayout.FieldRows / 2);

        buffer.Write(ScreenLayout.CenterX(ScreenLayout.FieldX, FieldWidth, title.Length), centerRow, title, _theme.Menu.Title);
        buffer.Write(ScreenLayout.CenterX(ScreenLayout.FieldX, FieldWidth, Hint.Length), centerRow + 2, Hint, _theme.Menu.Hint);
    }
}
