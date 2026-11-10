namespace Zmeika.App.Storage.Dto;

internal sealed record MenuDto(
    string Frame,
    string Title,
    string Item,
    string SelectedItem,
    string Marker,
    string Hint,
    string PlayerName,
    string InputText,
    string InputCursor);
