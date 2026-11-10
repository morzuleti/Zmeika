namespace Zmeika.Core.Players;

public sealed class PlayersBook
{
    private const int FirstLevel = 1;

    private readonly List<GameRecord> _games;
    private readonly List<PlayerProgress> _progress;

    public PlayersBook(IEnumerable<GameRecord> games, IEnumerable<PlayerProgress> progress)
    {
        ArgumentNullException.ThrowIfNull(games);
        ArgumentNullException.ThrowIfNull(progress);

        _games = [.. games];
        _progress = [.. progress];
        Validate();
    }

    public static PlayersBook Empty => new([], []);

    public IReadOnlyList<GameRecord> Games => _games;

    public IReadOnlyList<PlayerProgress> Progress => _progress;

    public PlayerProgress? FindProgress(PlayerName player)
    {
        ArgumentNullException.ThrowIfNull(player);
        return _progress.Find(entry => player.Matches(entry.Player));
    }

    public GameRecord FindGame(Guid gameId) =>
        _games.Find(game => game.Id == gameId)
        ?? throw new ArgumentException($"Unknown game {gameId}.", nameof(gameId));

    public GameRecord StartNewGame(PlayerName player, Guid gameId, DateTimeOffset startedAt, int initialLives)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(initialLives);

        if (FindProgress(player) is { } unfinished)
        {
            _progress.Remove(unfinished);
            _games.RemoveAll(game => game.Id == unfinished.GameId && game.Status == GameRecordStatus.InProgress);
        }

        GameRecord record = new(gameId, player.Value, Score: 0, startedAt, GameRecordStatus.InProgress);
        _games.Add(record);
        _progress.Add(new PlayerProgress(player.Value, gameId, FirstLevel, initialLives, Score: 0));
        return record;
    }

    public void RecordLevelStart(Guid gameId, int level, int lives, int score)
    {
        GameRecord game = FindGame(gameId);
        _progress.RemoveAll(entry => entry.GameId == gameId);
        _progress.Add(new PlayerProgress(game.Player, gameId, level, lives, score));
    }

    public void RecordScore(Guid gameId, int score)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(score);
        Replace(FindGame(gameId) with { Score = score });
    }

    public void Finish(Guid gameId, GameRecordStatus status)
    {
        if (status == GameRecordStatus.InProgress)
        {
            throw new ArgumentException("A finished game must be won or lost.", nameof(status));
        }

        Replace(FindGame(gameId) with { Status = status });
        _progress.RemoveAll(entry => entry.GameId == gameId);
    }

    public void Retry(Guid gameId, int level, int lives, int score)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(score);
        Replace(FindGame(gameId) with { Score = score, Status = GameRecordStatus.InProgress });
        RecordLevelStart(gameId, level, lives, score);
    }

    public IReadOnlyList<LeaderboardRow> Leaderboard(Guid currentGameId, int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        List<LeaderboardRow> ranked = [.. _games
            .OrderByDescending(game => game.Score)
            .ThenBy(game => game.StartedAt)
            .Select((game, index) => new LeaderboardRow(index + 1, game.Player, game.Score, game.Id == currentGameId))];

        List<LeaderboardRow> top = [.. ranked.Take(size)];
        if (!top.Exists(row => row.IsCurrent) && ranked.Find(row => row.IsCurrent) is { } current)
        {
            top.Add(current);
        }

        return top;
    }

    private void Replace(GameRecord updated)
    {
        int index = _games.FindIndex(game => game.Id == updated.Id);
        _games[index] = updated;
    }

    private void Validate()
    {
        if (_games.Select(game => game.Id).Distinct().Count() != _games.Count)
        {
            throw new ArgumentException("Game ids must be unique.");
        }

        if (_games.Exists(game => game.Score < 0 || string.IsNullOrWhiteSpace(game.Player)))
        {
            throw new ArgumentException("Every game needs a player name and a non-negative score.");
        }

        if (_progress.Exists(entry => entry.Level < 1 || entry.Lives < 1 || entry.Score < 0 || !_games.Exists(game => game.Id == entry.GameId)))
        {
            throw new ArgumentException("Every progress entry must point to an existing game and have a valid level, lives and score.");
        }
    }
}
