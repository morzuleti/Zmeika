namespace Zmeika.App.Rendering;

internal sealed record MenuTheme(
    Rgb Frame,
    Rgb Title,
    Rgb Item,
    Rgb SelectedItem,
    Rgb Marker,
    Rgb Hint,
    Rgb PlayerName,
    Rgb InputText,
    Rgb InputCursor);
