using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

public sealed class GameSession
{
    private readonly IRandomSource _random;

    private GameSession(GameRules rules, IRandomSource random, int level, int lives, int score)
    {
        Rules = rules;
        _random = random;
        Lives = lives;
        Score = score;
        Current = StartLevel(level);
    }

    public GameRules Rules { get; }

    public int Level { get; private set; }

    public int Lives { get; private set; }

    public int Score { get; private set; }

    public int ScoreAtLevelStart { get; private set; }

    public int LivesAtLevelStart { get; private set; }

    public LevelSession Current { get; private set; }

    public GameStatus Status { get; private set; }

    public bool IsLastLevel => Level == Rules.LevelCount;

    public static GameSession NewGame(GameRules rules, IRandomSource random)
    {
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(random);

        return new GameSession(rules, random, level: 1, rules.InitialLives, score: 0);
    }

    public static GameSession Resume(GameRules rules, IRandomSource random, int level, int lives, int score)
    {
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(random);
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(level, rules.LevelCount);
        ArgumentOutOfRangeException.ThrowIfLessThan(lives, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(lives, rules.MaxLives);
        ArgumentOutOfRangeException.ThrowIfNegative(score);

        return new GameSession(rules, random, level, lives, score);
    }

    public void QueueTurn(Direction direction)
    {
        if (Status == GameStatus.Playing)
        {
            Current.QueueTurn(direction);
        }
    }

    public IReadOnlyList<GameEvent> Update(TimeSpan elapsed)
    {
        if (Status != GameStatus.Playing)
        {
            return [];
        }

        List<GameEvent> events = [];
        foreach (LevelEvent levelEvent in Current.Update(elapsed))
        {
            Apply(levelEvent, events);
        }

        return events;
    }

    public void RestartLevel()
    {
        RequireStatus(GameStatus.Died);
        Current = StartLevel(Level);
    }

    public void AdvanceToNextLevel()
    {
        RequireStatus(GameStatus.LevelCompleted);
        Current = StartLevel(Level + 1);
    }

    public void RetryAfterGameOver()
    {
        RequireStatus(GameStatus.GameOver);
        Lives = Rules.InitialLives;
        Score = ScoreAtLevelStart;
        Current = StartLevel(Level);
    }

    private LevelSession StartLevel(int level)
    {
        Level = level;
        ScoreAtLevelStart = Score;
        LivesAtLevelStart = Lives;
        Status = GameStatus.Playing;
        return new LevelSession(Rules, level, _random);
    }

    private void Apply(LevelEvent levelEvent, List<GameEvent> events)
    {
        switch (levelEvent)
        {
            case AppleTaken apple:
                Score += Rules.PointsPerApple;
                events.Add(new AppleEaten(apple.Cell, Score, apple.ApplesEaten));
                break;
            case HeartTaken heart:
                Lives = Math.Min(Lives + 1, Rules.MaxLives);
                events.Add(new HeartEaten(heart.Cell, Lives));
                break;
            case HeartAppeared heart:
                events.Add(new HeartSpawned(heart.Cell));
                break;
            case HeartExpired heart:
                events.Add(new HeartGone(heart.Cell));
                break;
            case Crashed crash:
                Die(crash.Cause, events);
                break;
            case AllApplesTaken:
                Status = IsLastLevel ? GameStatus.Won : GameStatus.LevelCompleted;
                events.Add(new LevelCompleted(Level, IsLastLevel));
                break;
            default:
                throw new InvalidOperationException($"Unknown level event {levelEvent}.");
        }
    }

    private void Die(DeathCause cause, List<GameEvent> events)
    {
        Lives--;
        if (Lives > 0)
        {
            Score = ScoreAtLevelStart;
            Status = GameStatus.Died;
            events.Add(new SnakeDied(cause, Lives));
            return;
        }

        Status = GameStatus.GameOver;
        events.Add(new SnakeDied(cause, Lives));
        events.Add(new GameOver(Score));
    }

    private void RequireStatus(GameStatus expected)
    {
        if (Status != expected)
        {
            throw new InvalidOperationException($"The game must be in status {expected}, but it is {Status}.");
        }
    }
}
