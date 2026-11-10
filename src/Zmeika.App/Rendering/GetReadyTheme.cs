namespace Zmeika.App.Rendering;

internal sealed record GetReadyTheme(IReadOnlyList<Rgb> LetterColors, Rgb CountdownCurrent, Rgb CountdownOther);
