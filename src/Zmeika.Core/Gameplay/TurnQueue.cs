using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

internal sealed class TurnQueue
{
    private const int Capacity = 2;

    private readonly Queue<Direction> _pending = new(Capacity);
    private Direction _newest;

    public int Count => _pending.Count;

    public void Enqueue(Direction direction, Direction current)
    {
        Direction last = _pending.Count > 0 ? _newest : current;
        if (_pending.Count == Capacity || direction == last || direction.IsOpposite(last))
        {
            return;
        }

        _pending.Enqueue(direction);
        _newest = direction;
    }

    public bool TryDequeue(out Direction direction) => _pending.TryDequeue(out direction);
}
