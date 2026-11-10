namespace Zmeika.App.Rendering;

internal sealed class FrameBuffer
{
    private readonly Cell[,] _cells;

    public FrameBuffer(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        Width = width;
        Height = height;
        _cells = new Cell[height, width];
        Clear();
    }

    public int Width { get; }

    public int Height { get; }

    public Cell this[int x, int y] => _cells[y, x];

    public void Clear()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                _cells[y, x] = Cell.Empty;
            }
        }
    }

    public void Put(int x, int y, char glyph, Rgb? foreground = null, Rgb? background = null)
    {
        if (Contains(x, y))
        {
            _cells[y, x] = new Cell(glyph, foreground, background);
        }
    }

    public void Write(int x, int y, string text, Rgb? foreground = null, Rgb? background = null)
    {
        ArgumentNullException.ThrowIfNull(text);

        for (var i = 0; i < text.Length; i++)
        {
            Put(x + i, y, text[i], foreground, background);
        }
    }

    public void WriteCentered(int y, string text, Rgb? foreground = null) =>
        Write(ScreenLayout.CenterX(0, Width, text.Length), y, text, foreground);

    public void FillBackground(int x, int y, int width, int height, Rgb background)
    {
        for (var row = y; row < y + height; row++)
        {
            for (var column = x; column < x + width; column++)
            {
                Put(column, row, ' ', null, background);
            }
        }
    }

    private bool Contains(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
}
