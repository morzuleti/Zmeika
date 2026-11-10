using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;
using Zmeika.Core.Model;

namespace Zmeika.App.Screens;

internal sealed class LevelScreen(GameContext context, GameSession session) : IScreen
{
    private static readonly TimeSpan s_deathPause = TimeSpan.FromSeconds(1);

    private TimeSpan? _deathPauseLeft;
    private string? _finalMessage;

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (_finalMessage is not null)
        {
            if (GameKeys.IsBack(key))
            {
                context.Navigator.Exit();
            }

            return;
        }

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
        if (_finalMessage is not null)
        {
            return;
        }

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
                _finalMessage = "YOU WIN!";
                break;
            case GameStatus.Playing:
            case GameStatus.Died:
            case GameStatus.GameOver:
                break;
            default:
                throw new InvalidOperationException($"Unknown game status {session.Status}.");
        }
    }

    public void Paint(FrameBuffer buffer)
    {
        LevelView.Paint(buffer, context, session);
        if (_finalMessage is not null)
        {
            PaintFinalMessage(buffer, _finalMessage);
        }
    }

    private void AdvanceDeathPause(TimeSpan left)
    {
        if (left > TimeSpan.Zero)
        {
            _deathPauseLeft = left;
            return;
        }

        _deathPauseLeft = null;
        if (session.Status == GameStatus.GameOver)
        {
            _finalMessage = "GAME OVER";
            return;
        }

        context.Navigator.Show(new OopsScreen(context, session));
    }

    private void PaintFinalMessage(FrameBuffer buffer, string title)
    {
        const string Hint = "Esc exit";
        int centerRow = ScreenLayout.FieldY + (ScreenLayout.FieldRows / 2);
        MenuTheme menu = context.Theme.Menu;

        buffer.Write(ScreenLayout.CenterX(ScreenLayout.FieldX, ScreenLayout.FieldWidth, title.Length), centerRow, title, menu.Title);
        buffer.Write(ScreenLayout.CenterX(ScreenLayout.FieldX, ScreenLayout.FieldWidth, Hint.Length), centerRow + 2, Hint, menu.Hint);
    }
}
