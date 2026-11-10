namespace Zmeika.App.Rendering;

internal sealed class FireworksRocket(int column, double row, double burstRow, Rgb burstColor)
{
    public int Column { get; } = column;

    public double Row { get; set; } = row;

    public double BurstRow { get; } = burstRow;

    public Rgb BurstColor { get; } = burstColor;
}
