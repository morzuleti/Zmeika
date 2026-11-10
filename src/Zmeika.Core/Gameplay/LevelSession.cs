using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

public sealed class LevelSession
{
    private readonly GameRules _rules;
    private readonly LevelRules _levelRules;
    private readonly IRandomSource _random;
    private readonly TurnQueue _turns = new();
    private TimeSpan _stepAccumulator;
    private TimeSpan _heartDelay;

    public LevelSession(GameRules rules, int level, IRandomSource random)
    {
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(random);
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(level, rules.LevelCount);

        _rules = rules;
        _levelRules = rules.Levels[level - 1];
        _random = random;

        Level = level;
        ApplesRequired = _levelRules.ApplesRequired;
        Snake = CreateStartSnake(rules.InitialLength);
        Apple = PlaceApple();
        _heartDelay = DrawHeartDelay();
    }

    public static FieldSize Field => GameRules.Field;

    public int Level { get; }

    public Snake Snake { get; }

    public Apple Apple { get; private set; }

    public Position? Heart { get; private set; }

    public int ApplesEaten { get; private set; }

    public int ApplesRequired { get; }

    public LevelStatus Status { get; private set; } = LevelStatus.Playing;

    public void QueueTurn(Direction direction)
    {
        if (Status == LevelStatus.Playing)
        {
            _turns.Enqueue(direction, Snake.Direction);
        }
    }

    public IReadOnlyList<LevelEvent> Update(TimeSpan elapsed)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(elapsed, TimeSpan.Zero);

        if (Status != LevelStatus.Playing)
        {
            return [];
        }

        List<LevelEvent> events = [];
        AdvanceHeartTimer(elapsed, events);

        _stepAccumulator += elapsed;
        while (Status == LevelStatus.Playing && _stepAccumulator >= _levelRules.StepInterval)
        {
            _stepAccumulator -= _levelRules.StepInterval;
            Step(events);
        }

        return events;
    }

    private static Snake CreateStartSnake(int length)
    {
        int column = Field.Columns - 1;
        int headRow = Field.Rows - length;
        IEnumerable<Position> segments = Enumerable.Range(headRow, length).Select(row => new Position(column, row));
        return new Snake(segments, Direction.Up);
    }

    private void AdvanceHeartTimer(TimeSpan elapsed, List<LevelEvent> events)
    {
        if (Heart is not null)
        {
            return;
        }

        _heartDelay -= elapsed;
        if (_heartDelay > TimeSpan.Zero)
        {
            return;
        }

        Position? cell = PickFreeCell(Apple.Cell);
        if (cell is { } heart)
        {
            Heart = heart;
            events.Add(new HeartAppeared(heart));
        }
    }

    private void Step(List<LevelEvent> events)
    {
        Direction direction = _turns.TryDequeue(out Direction turn) ? turn : Snake.Direction;
        Position next = Snake.NextHead(direction);

        if (!Field.Contains(next))
        {
            Crash(DeathCause.Wall, events);
            return;
        }

        bool grows = next == Apple.Cell;
        if (Snake.WouldBiteItself(next, grows))
        {
            Crash(DeathCause.Body, events);
            return;
        }

        Snake.MoveTo(next, direction, grows);

        if (next == Heart)
        {
            Heart = null;
            _heartDelay = DrawHeartDelay();
            events.Add(new HeartTaken(next));
        }

        if (grows)
        {
            EatApple(next, events);
        }
    }

    private void EatApple(Position cell, List<LevelEvent> events)
    {
        ApplesEaten++;
        events.Add(new AppleTaken(cell, ApplesEaten));

        if (ApplesEaten >= ApplesRequired)
        {
            Status = LevelStatus.Completed;
            events.Add(new AllApplesTaken());
            return;
        }

        Apple = PlaceApple();
    }

    private void Crash(DeathCause cause, List<LevelEvent> events)
    {
        Status = LevelStatus.Died;
        events.Add(new Crashed(cause));
    }

    private Apple PlaceApple()
    {
        Position cell = PickFreeCell(excluded: null)
            ?? throw new InvalidOperationException("There is no free cell for a new apple.");
        return new Apple(cell, _random.NextIndex(_rules.AppleVariantCount));
    }

    private Position? PickFreeCell(Position? excluded)
    {
        List<Position> free = [];
        foreach (Position cell in Field.AllCells())
        {
            if (cell != excluded && cell != Heart && !Snake.Occupies(cell))
            {
                free.Add(cell);
            }
        }

        return free.Count == 0 ? null : free[_random.NextIndex(free.Count)];
    }

    private TimeSpan DrawHeartDelay() => _random.NextDuration(_rules.HeartSpawnMin, _rules.HeartSpawnMax);
}
