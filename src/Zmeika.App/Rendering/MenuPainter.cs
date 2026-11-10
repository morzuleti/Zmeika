namespace Zmeika.App.Rendering;

internal static class MenuPainter
{
    private const int ItemOffsetX = 5;
    private const int MarkerOffsetX = ItemOffsetX - 2;

    public static void Paint(FrameBuffer buffer, int dialogX, int y, IReadOnlyList<string> items, int selectedIndex, MenuTheme theme)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(theme);

        for (var i = 0; i < items.Count; i++)
        {
            bool isSelected = i == selectedIndex;
            if (isSelected)
            {
                buffer.Put(dialogX + MarkerOffsetX, y + i, Glyphs.MenuMarker, theme.Marker);
            }

            buffer.Write(dialogX + ItemOffsetX, y + i, items[i], isSelected ? theme.SelectedItem : theme.Item);
        }
    }
}
