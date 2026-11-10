using System.Globalization;
using System.Text;

namespace Zmeika.App.Rendering;

internal static class AnsiFormatter
{
    private const string Reset = "\e[0m";
    private const string DefaultForeground = "\e[39m";
    private const string DefaultBackground = "\e[49m";

    public static string Format(FrameBuffer frame) => FormatChanges(frame, previous: null);

    public static string FormatChanges(FrameBuffer frame, FrameBuffer? previous)
    {
        ArgumentNullException.ThrowIfNull(frame);

        StringBuilder builder = new();
        for (var y = 0; y < frame.Height; y++)
        {
            AppendChangedRuns(builder, frame, previous, y);
        }

        if (builder.Length > 0)
        {
            builder.Append(Reset);
        }

        return builder.ToString();
    }

    private static void AppendChangedRuns(StringBuilder builder, FrameBuffer frame, FrameBuffer? previous, int y)
    {
        Cell? last = null;
        for (var x = 0; x < frame.Width; x++)
        {
            Cell cell = frame[x, y];
            if (previous is not null && previous[x, y] == cell)
            {
                last = null;
                continue;
            }

            if (last is null)
            {
                builder.Append(CultureInfo.InvariantCulture, $"\e[{y + 1};{x + 1}H");
            }

            if (last is null || last.Value.Foreground != cell.Foreground)
            {
                AppendForeground(builder, cell.Foreground);
            }

            if (last is null || last.Value.Background != cell.Background)
            {
                AppendBackground(builder, cell.Background);
            }

            builder.Append(cell.Glyph);
            last = cell;
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
