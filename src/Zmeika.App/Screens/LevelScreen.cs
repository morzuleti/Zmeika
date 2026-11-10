using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;
using Zmeika.Core.Model;

namespace Zmeika.App.Screens;

internal sealed class LevelScreen : IScreen
{
    private const int FirstLevel = 1;

    private readonly GameRules _rules;
    private readonly Theme _theme;
    private readonly IRandomSource _random;
    private readonly INavigator _navigator;
    private readonly KeyBindings _keys = KeyBindings.Default;
    private LevelSession _session;

    public LevelScreen(GameRules rules, Theme theme, IRandomSource random, INavigator navigator)
    {
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentNullException.ThrowIfNull(random);
        ArgumentNullException.ThrowIfNull(navigator);

        _rules = rules;
        _theme = theme;
        _random = random;
        _navigator = navigator;
        _session = StartLevel();
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
        if (_session.Status != LevelStatus.Playing)
        {
            _session = StartLevel();
        }
    }

    public void Paint(FrameBuffer buffer)
    {
        FieldStyle style = new(
            _theme.Levels[_session.Level - 1],
            _theme.DefaultSkin,
            _theme.DefaultSnakeColor.Color,
            _theme.AppleColors,
            _theme.Heart);
        FieldPainter.Paint(buffer, _session, style);
    }

    private static bool IsExitKey(ConsoleKeyInfo key) =>
        key.Key == ConsoleKey.Escape
        || (key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control));

    private LevelSession StartLevel() => new(_rules, FirstLevel, _random);
}
