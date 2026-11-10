using System.Globalization;

namespace Zmeika.App.Rendering;

internal static class CountdownPainter
{
    private static readonly string s_separator = $" {Glyphs.MiddleDot} ";

    public static void Paint(FrameBuffer buffer, int areaX, int areaWidth, int y, int total, int current, GetReadyTheme theme)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentOutOfRangeException.ThrowIfLessThan(total, 1);

        string[] digits = [.. Enumerable.Range(1, total).Reverse().Select(digit => digit.ToString(CultureInfo.InvariantCulture))];
        int width = digits.Sum(digit => digit.Length) + ((digits.Length - 1) * s_separator.Length);

        int x = ScreenLayout.CenterX(areaX, areaWidth, width);
        for (var i = 0; i < digits.Length; i++)
        {
            if (i > 0)
            {
                buffer.Write(x, y, s_separator, theme.CountdownOther);
                x += s_separator.Length;
            }

            bool isCurrent = total - i == current;
            buffer.Write(x, y, digits[i], isCurrent ? theme.CountdownCurrent : theme.CountdownOther);
            x += digits[i].Length;
        }
    }
}
