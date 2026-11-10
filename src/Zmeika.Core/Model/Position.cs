namespace Zmeika.Core.Model;

public readonly record struct Position(int X, int Y)
{
    public Position Offset(int dx, int dy) => new(X + dx, Y + dy);
}
