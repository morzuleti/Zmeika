using Zmeika.Core.Model;

namespace Zmeika.App.Input;

internal sealed record KeyBindings(ConsoleKey Up, ConsoleKey Down, ConsoleKey Left, ConsoleKey Right)
{
    public static KeyBindings Default { get; } =
        new(ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow);

    public static bool IsReserved(ConsoleKey key) => key is ConsoleKey.Spacebar or ConsoleKey.Escape;

    public ConsoleKey KeyFor(Direction direction) => direction switch
    {
        Direction.Up => Up,
        Direction.Down => Down,
        Direction.Left => Left,
        Direction.Right => Right,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown direction."),
    };

    public KeyBindings With(Direction direction, ConsoleKey key) => direction switch
    {
        Direction.Up => this with { Up = key },
        Direction.Down => this with { Down = key },
        Direction.Left => this with { Left = key },
        Direction.Right => this with { Right = key },
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown direction."),
    };

    public bool IsValid()
    {
        ConsoleKey[] keys = [Up, Down, Left, Right];
        return keys.Distinct().Count() == keys.Length && !keys.Any(IsReserved) && keys.All(Enum.IsDefined);
    }

    public bool TryGetDirection(ConsoleKey key, out Direction direction)
    {
        if (key == Up)
        {
            direction = Direction.Up;
        }
        else if (key == Down)
        {
            direction = Direction.Down;
        }
        else if (key == Left)
        {
            direction = Direction.Left;
        }
        else if (key == Right)
        {
            direction = Direction.Right;
        }
        else
        {
            direction = default;
            return false;
        }

        return true;
    }
}
