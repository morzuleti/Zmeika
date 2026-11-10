using System.Text;

namespace Zmeika.App.Rendering;

internal static class PlainTextFormatter
{
    public static string Format(FrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        StringBuilder builder = new();
        for (var y = 0; y < buffer.Height; y++)
        {
            StringBuilder row = new(buffer.Width);
            for (var x = 0; x < buffer.Width; x++)
            {
                row.Append(buffer[x, y].Glyph);
            }

            builder.AppendLine(row.ToString().TrimEnd());
        }

        return builder.ToString();
    }
}
