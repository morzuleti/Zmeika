using Zmeika.App.Storage;
using Zmeika.Core.Gameplay;
using Zmeika.Core.Players;

namespace Zmeika.App.Screens;

internal sealed class GameFlow(GameRules rules, IRandomSource random, PlayersStore players, TimeProvider clock)
{
    private Guid? _gameId;

    public PlayerName? Player { get; private set; }

    public bool LastSaveFailed { get; private set; }

    public PlayerProgress? FindProgress(PlayerName player) => players.Book.FindProgress(player);

    public GameSession StartNewGame(PlayerName player)
    {
        ArgumentNullException.ThrowIfNull(player);

        Guid gameId = Guid.NewGuid();
        players.Book.StartNewGame(player, gameId, clock.GetLocalNow(), rules.InitialLives);
        Player = player;
        _gameId = gameId;
        Save();
        return GameSession.NewGame(rules, random);
    }

    public GameSession ContinueGame(PlayerName player, PlayerProgress progress)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(progress);

        if (!CanResume(progress))
        {
            return StartNewGame(player);
        }

        Player = player;
        _gameId = progress.GameId;
        return GameSession.Resume(rules, random, progress.Level, progress.Lives, progress.Score);
    }

    public GameSession StartNewGameForCurrentPlayer() =>
        StartNewGame(Player ?? throw new InvalidOperationException("No player has been chosen yet."));

    public void OnLevelStarted(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (_gameId is { } gameId)
        {
            players.Book.RecordLevelStart(gameId, session.Level, session.LivesAtLevelStart, session.ScoreAtLevelStart);
            Save();
        }
    }

    public void OnEvents(GameSession session, IReadOnlyList<GameEvent> events)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(events);

        if (_gameId is not { } gameId || events.Count == 0)
        {
            return;
        }

        bool changed = false;
        if (events.Any(e => e is AppleEaten or SnakeDied))
        {
            players.Book.RecordScore(gameId, session.Score);
            changed = true;
        }

        if (events.OfType<GameOver>().Any())
        {
            players.Book.Finish(gameId, GameRecordStatus.Lost);
            changed = true;
        }

        if (events.OfType<LevelCompleted>().Any(completed => completed.IsLastLevel))
        {
            players.Book.Finish(gameId, GameRecordStatus.Won);
            changed = true;
        }

        if (changed)
        {
            Save();
        }
    }

    private bool CanResume(PlayerProgress progress) =>
        progress.Level >= 1 && progress.Level <= rules.LevelCount
        && progress.Lives >= 1 && progress.Lives <= rules.MaxLives
        && progress.Score >= 0;

    private void Save()
    {
        try
        {
            players.Save();
            LastSaveFailed = false;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            LastSaveFailed = true;
        }
    }
}
