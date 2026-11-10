namespace Zmeika.App.Rendering;

internal sealed record LevelColorScheme(int Level, string Name, Rgb Wall, Rgb ShadeA, Rgb ShadeB)
{
    public Rgb ShadeAt(int column, int row) => (column + row) % 2 == 0 ? ShadeA : ShadeB;
}
