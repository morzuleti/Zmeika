namespace Zmeika.App.Storage.Dto;

internal sealed record HudDto(
    string Frame,
    string Level,
    string Label,
    string PlayerName,
    string Heart,
    string Apple,
    string Value,
    string Score,
    string ControlsCaption,
    string ControlDirection,
    string ControlKey);
