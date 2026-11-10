namespace Zmeika.App.Input;

internal static class GameKeys
{
    public static bool IsPause(ConsoleKeyInfo key) =>
        key.Key is ConsoleKey.Spacebar or ConsoleKey.Escape || IsControlC(key);

    public static bool IsConfirm(ConsoleKeyInfo key) => key.Key == ConsoleKey.Enter;

    public static bool IsBack(ConsoleKeyInfo key) => key.Key == ConsoleKey.Escape;

    public static bool IsMenuUp(ConsoleKeyInfo key) => key.Key == ConsoleKey.UpArrow;

    public static bool IsMenuDown(ConsoleKeyInfo key) => key.Key == ConsoleKey.DownArrow;

    private static bool IsControlC(ConsoleKeyInfo key) =>
        key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control);
}
