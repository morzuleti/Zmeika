namespace Zmeika.Core.Model;

public sealed class Snake
{
    private readonly List<Position> _segments;
    private readonly HashSet<Position> _occupied;

    public Snake(IEnumerable<Position> segmentsFromHead, Direction direction)
    {
        ArgumentNullException.ThrowIfNull(segmentsFromHead);

        _segments = [.. segmentsFromHead];
        if (_segments.Count == 0)
        {
            throw new ArgumentException("A snake needs at least one segment.", nameof(segmentsFromHead));
        }

        _occupied = [.. _segments];
        if (_occupied.Count != _segments.Count)
        {
            throw new ArgumentException("Snake segments must not overlap.", nameof(segmentsFromHead));
        }

        Direction = direction;
    }

    public Position Head => _segments[0];

    public Position Tail => _segments[^1];

    public IReadOnlyList<Position> Segments => _segments;

    public Direction Direction { get; private set; }

    public int Length => _segments.Count;

    public Position NextHead(Direction direction) => Head.Step(direction);

    public bool WouldBiteItself(Position nextHead, bool grows) =>
        _occupied.Contains(nextHead) && (grows || nextHead != Tail);

    public void MoveTo(Position nextHead, Direction direction, bool grows)
    {
        if (WouldBiteItself(nextHead, grows))
        {
            throw new InvalidOperationException($"The snake cannot move into its own body at {nextHead}.");
        }

        if (!grows)
        {
            _occupied.Remove(Tail);
            _segments.RemoveAt(_segments.Count - 1);
        }

        _segments.Insert(0, nextHead);
        _occupied.Add(nextHead);
        Direction = direction;
    }

    public bool Occupies(Position position) => _occupied.Contains(position);
}
