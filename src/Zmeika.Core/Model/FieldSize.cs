namespace Zmeika.Core.Model;

public readonly record struct FieldSize(int Columns, int Rows)
{
    public int CellCount => Columns * Rows;

    public bool Contains(Position position) =>
        position.X >= 0 && position.X < Columns && position.Y >= 0 && position.Y < Rows;

    public IEnumerable<Position> AllCells()
    {
        for (var y = 0; y < Rows; y++)
        {
            for (var x = 0; x < Columns; x++)
            {
                yield return new Position(x, y);
            }
        }
    }
}
