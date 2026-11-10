using System.Collections.Frozen;
using System.Globalization;
using Zmeika.App.Rendering;

namespace Zmeika.App.Storage;

internal sealed class ColorResolver
{
    private readonly FrozenDictionary<string, Rgb> _palette;

    public ColorResolver(IReadOnlyList<NamedColor> palette)
    {
        ArgumentNullException.ThrowIfNull(palette);

        _palette = palette.ToFrozenDictionary(color => color.Name, color => color.Color, StringComparer.OrdinalIgnoreCase);
    }

    public Rgb Resolve(string value, string path)
    {
        if (_palette.TryGetValue(value, out Rgb named))
        {
            return named;
        }

        if (Rgb.TryFromHex(value, out Rgb hex))
        {
            return hex;
        }

        throw new ConfigException($"{path}: unknown colour '{value}'. Use a name from 'colors' or #RRGGBB.");
    }

    public IReadOnlyList<Rgb> ResolveAll(IReadOnlyList<string> values, string path)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Count == 0)
        {
            throw new ConfigException($"{path}: the list must contain at least one colour.");
        }

        Rgb[] colors = new Rgb[values.Count];
        for (var i = 0; i < values.Count; i++)
        {
            colors[i] = Resolve(values[i], string.Create(CultureInfo.InvariantCulture, $"{path}[{i}]"));
        }

        return colors;
    }
}
