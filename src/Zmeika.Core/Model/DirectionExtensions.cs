namespace Zmeika.Core.Model;

public static class DirectionExtensions
{
    public static (int Dx, int Dy) Delta(this Direction direction) => direction switch
    {
        Direction.Up => (0, -1),
        Direction.Down => (0, 1),
        Direction.Left => (-1, 0),
        Direction.Right => (1, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown direction."),
    };

    public static bool IsOpposite(this Direction direction, Direction other) => (direction, other) switch
    {
        (Direction.Up, Direction.Down) or (Direction.Down, Direction.Up) => true,
        (Direction.Left, Direction.Right) or (Direction.Right, Direction.Left) => true,
        _ => false,
    };
}
