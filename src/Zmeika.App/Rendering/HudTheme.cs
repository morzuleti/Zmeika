namespace Zmeika.App.Rendering;

internal sealed record HudTheme(
    Rgb Frame,
    Rgb Level,
    Rgb Label,
    Rgb PlayerName,
    Rgb Heart,
    Rgb Apple,
    Rgb Value,
    Rgb Score,
    Rgb ControlsCaption,
    Rgb ControlDirection,
    Rgb ControlKey);
