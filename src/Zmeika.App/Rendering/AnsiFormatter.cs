using System.Globalization;
using System.Text;

namespace Zmeika.App.Rendering;

internal static class AnsiFormatter
{
    private const string Reset = "\e[0m";
    private const string DefaultForeground = "\e[39m";
    private const string DefaultBackground = "\e[49m";

    public static string Format(FrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        StringBuilder builder = new(buffer.Width * buffer.Height * 8);
        for (var y = 0; y < buffer.Height; y++)
        {
            builder.Append(CultureInfo.InvariantCulture, $"\e[{y + 1};1H");
            AppendRow(builder, buffer, y);
        }

        builder.Append(Reset);
        return builder.ToString();
    }

    private static void AppendRow(StringBuilder builder, FrameBuffer buffer, int y)
    {
        Cell? previous = null;
        for (var x = 0; x < buffer.Width; x++)
        {
            Cell cell = buffer[x, y];
            if (previous is null || previous.Value.Foreground != cell.Foreground)
            {
                AppendForeground(builder, cell.Foreground);
            }

            if (previous is null || previous.Value.Background != cell.Background)
            {
                AppendBackground(builder, cell.Background);
            }

            builder.Append(cell.Glyph);
            previous = cell;
        }
    }

    private static void AppendForeground(StringBuilder builder, Rgb? color)
    {
        if (color is { } rgb)
        {
            builder.Append(CultureInfo.InvariantCulture, $"\e[38;2;{rgb.R};{rgb.G};{rgb.B}m");
        }
        else
        {
            builder.Append(DefaultForeground);
        }
    }

    private static void AppendBackground(StringBuilder builder, Rgb? color)
    {
        if (color is { } rgb)
        {
            builder.Append(CultureInfo.InvariantCulture, $"\e[48;2;{rgb.R};{rgb.G};{rgb.B}m");
        }
        else
        {
            builder.Append(DefaultBackground);
        }
    }
}
