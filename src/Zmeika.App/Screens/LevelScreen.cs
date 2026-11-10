using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;
using Zmeika.Core.Model;

namespace Zmeika.App.Screens;

internal sealed class LevelScreen(GameContext context, GameSession session) : IScreen
{
    private static readonly TimeSpan s_deathPause = TimeSpan.FromSeconds(1);

    private TimeSpan? _deathPauseLeft;

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (_deathPauseLeft is not null)
        {
            return;
        }

        if (GameKeys.IsPause(key))
        {
            context.Navigator.Show(new PauseMenuScreen(context, session, this));
            return;
        }

        if (context.Keys.TryGetDirection(key.Key, out Direction direction))
        {
            session.QueueTurn(direction);
        }
    }

    public void Update(TimeSpan elapsed)
    {
        if (_deathPauseLeft is { } left)
        {
            AdvanceDeathPause(left - elapsed);
            return;
        }

        IReadOnlyList<GameEvent> events = session.Update(elapsed);
        context.Flow.OnEvents(session, events);
        if (events.OfType<SnakeDied>().Any())
        {
            _deathPauseLeft = s_deathPause;
            return;
        }

        switch (session.Status)
        {
            case GameStatus.LevelCompleted:
                session.AdvanceToNextLevel();
                context.Navigator.Show(new GetReadyScreen(context, session));
                break;
            case GameStatus.Won:
                context.Navigator.Show(LeaderboardScreen.AfterGame(context, session, isWin: true));
                break;
            case GameStatus.Playing:
            case GameStatus.Died:
            case GameStatus.GameOver:
                break;
            default:
                throw new InvalidOperationException($"Unknown game status {session.Status}.");
        }
    }

    public void Paint(FrameBuffer buffer) => LevelView.Paint(buffer, context, session);

    private void AdvanceDeathPause(TimeSpan left)
    {
        if (left > TimeSpan.Zero)
        {
            _deathPauseLeft = left;
            return;
        }

        _deathPauseLeft = null;
        IScreen next = session.Status == GameStatus.GameOver
            ? LeaderboardScreen.AfterGame(context, session, isWin: false)
            : new OopsScreen(context, session);
        context.Navigator.Show(next);
    }
}
