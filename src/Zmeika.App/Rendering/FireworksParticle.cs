namespace Zmeika.App.Rendering;

internal sealed class FireworksParticle(double column, double row, double columnsPerSecond, double rowsPerSecond, Rgb color)
{
    public double Column { get; set; } = column;

    public double Row { get; set; } = row;

    public double ColumnsPerSecond { get; } = columnsPerSecond;

    public double RowsPerSecond { get; set; } = rowsPerSecond;

    public Rgb Color { get; } = color;

    public TimeSpan Age { get; set; }
}
