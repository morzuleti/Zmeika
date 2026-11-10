using System.Globalization;
using Zmeika.Core.Players;

namespace Zmeika.App.Rendering;

internal static class LeaderboardPainter
{
    public const int Width = 31;

    private const int RowOffsetX = 2;
    private const int NameWidth = 18;

    public static void Paint(FrameBuffer buffer, int x, int y, IReadOnlyList<LeaderboardRow> rows, LeaderboardTheme theme)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(theme);

        buffer.Write(x + RowOffsetX, y, "#   Name                 Score", theme.Header);
        for (var i = 0; i < rows.Count; i++)
        {
            LeaderboardRow row = rows[i];
            Rgb color = row switch
            {
                { IsCurrent: true } => theme.CurrentGame,
                { Place: 1 } => theme.FirstPlace,
                _ => theme.OtherGames,
            };

            int rowY = y + 1 + i;
            if (row.IsCurrent)
            {
                buffer.Put(x, rowY, Glyphs.MenuMarker, color);
            }

            string name = row.Player.Length <= NameWidth ? row.Player : row.Player[..NameWidth];
            buffer.Write(x + RowOffsetX, rowY, string.Create(CultureInfo.InvariantCulture, $"{row.Place,-3} {name,-NameWidth} {row.Score,6}"), color);
        }
    }
}
