using System.Globalization;

namespace Zmeika.App.Rendering;

internal readonly record struct Rgb(byte R, byte G, byte B)
{
    private const int HexDigitCount = 6;

    public static Rgb FromHex(string hex) =>
        TryFromHex(hex, out Rgb color)
            ? color
            : throw new FormatException($"Colour '{hex}' must have the form #RRGGBB.");

    public static bool TryFromHex(string? hex, out Rgb color)
    {
        color = default;
        if (hex is null || !hex.StartsWith('#'))
        {
            return false;
        }

        ReadOnlySpan<char> digits = hex.AsSpan(1);
        if (digits.Length != HexDigitCount
            || !TryParseByte(digits[..2], out byte red)
            || !TryParseByte(digits[2..4], out byte green)
            || !TryParseByte(digits[4..], out byte blue))
        {
            return false;
        }

        color = new Rgb(red, green, blue);
        return true;
    }

    public string ToHex() => string.Create(CultureInfo.InvariantCulture, $"#{R:X2}{G:X2}{B:X2}");

    private static bool TryParseByte(ReadOnlySpan<char> digits, out byte value) =>
        byte.TryParse(digits, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out value);
}
